using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.ShaderData;

namespace Fulbo.Match
{
    [RequireComponent(typeof(Match))]
    public class TurnManager : MonoBehaviour
    {
        #region Enumerators
        public enum Phases
        {
            Teammates = 0,
            Dribbler,
            Opponents
        }
        #endregion

        [SerializeField] private float speed;

        private int turn = 0;

        private Match match;
        private MatchPlayer selectedPlayer;

        private Dictionary<Phases, List<MatchPlayer>> phasePlayers;

        private Action processActionOnPhaseEnd;

        private float WaitInterval => 1f / speed;
        private Player Player => match.Player;

        private event Action<MatchPlayer> PlayerActionConfirmedEvent;

        public event Action<MatchPlayer> PassEvent;
        public event Action ShotEvent;
        public event Action<Phases> PhaseEndedEvent;
        public event Action<int> TurnEndedEvent;

        private void Awake()
        {
            match = GetComponent<Match>();

            match.InitialPlayerSetEvent += OnInitialPlayerSet;
            match.MatchStartEvent += OnMatchStart;
            match.MatchEndEvent += OnMatchEnd;

            match.Ball.DribblerSetEvent += OnDribblerSet;

            phasePlayers = new Dictionary<Phases, List<MatchPlayer>>
            {
                [Phases.Teammates] = null,
                [Phases.Dribbler ] = null,
                [Phases.Opponents] = null
            };
        }

        private void Play()
        {
            InitializePhasePlayers(selectedPlayer);
            StartCoroutine(Turn());
        }

        private void Stop() => StopCoroutine(Turn());

        private void InitializePhasePlayers(MatchPlayer selectedPlayer)
        {
            this.selectedPlayer = selectedPlayer;

            phasePlayers[Phases.Teammates] = selectedPlayer.Team.GetPlayers(new MatchPlayer[] { selectedPlayer });
            phasePlayers[Phases.Opponents] = selectedPlayer.Rival.Players;
        }

        private IEnumerator Turn()
        {
            while (true)
            {
                for (int i = 0; i < phasePlayers.Count; i++) yield return StartCoroutine(Phase((Phases)i, phasePlayers[(Phases)i]));

                turn++;

                TurnEndedEvent?.Invoke(turn);
            }
        }

        private IEnumerator Phase(Phases phase, List<MatchPlayer> players)
        {
            // Wait for all players to select an action
            Dictionary<PlayerID, bool> results = new Dictionary<PlayerID, bool>();
            PlayerActionConfirmedEvent += (player) => { if (results.ContainsKey(player.ID)) results[player.ID] = true; };

            // Ask the players to choose an action
            foreach (MatchPlayer player in players)
            {
                player.Brain.ActionConfirmedEvent += OnPlayerActionConfirmed;
                results.Add(player.ID, false);

                player.Brain.ChooseAction();
            }

            yield return new WaitUntil(() => results.Values.All(result => result));

            yield return new WaitForSeconds(WaitInterval);

            foreach (MatchPlayer player in players) player.Brain.ExecuteAction();

            PlayerActionConfirmedEvent = null;

            processActionOnPhaseEnd?.Invoke();
            processActionOnPhaseEnd = null;

            Debug.Log(phase);
            PhaseEndedEvent?.Invoke(phase);

            yield return new WaitForSeconds(WaitInterval);
        }

        #region Handlers
        private void OnInitialPlayerSet(MatchPlayer initialPlayer) => selectedPlayer = initialPlayer;

        private void OnMatchStart() => Play();

        private void OnMatchEnd()
        {
            Stop();

            match.InitialPlayerSetEvent -= OnInitialPlayerSet;
            match.MatchStartEvent -= OnMatchStart;
            match.MatchEndEvent -= OnMatchEnd;

            match.Ball.DribblerSetEvent -= OnDribblerSet;
        }

        private void OnDribblerSet(MatchPlayer dribbler)
        {
            if (!dribbler) return;
            if (dribbler.Side != Player.Side) return;

            if (phasePlayers[Phases.Teammates] != null)
            {
                phasePlayers[Phases.Teammates].Remove(dribbler);
                if (selectedPlayer)
                {
                    selectedPlayer.Brain.GetAction<MPA_Pass>().PassEvent -= OnPass;
                    selectedPlayer.Brain.GetAction<MPA_Shoot>().ShotEvent -= OnShot;

                    phasePlayers[Phases.Teammates].Add(selectedPlayer);
                }
            }

            selectedPlayer = dribbler;
            phasePlayers[Phases.Dribbler] = new List<MatchPlayer>() { selectedPlayer };

            selectedPlayer.Brain.GetAction<MPA_Pass>().PassEvent += OnPass;
            selectedPlayer.Brain.GetAction<MPA_Shoot>().ShotEvent += OnShot;
        }

        private void OnPass(MatchPlayer receiver) => PassEvent?.Invoke(receiver);

        private void OnShot() => ShotEvent?.Invoke();

        private void OnPlayerActionConfirmed(MatchPlayer player)
        {
            player.Brain.ActionConfirmedEvent -= OnPlayerActionConfirmed;
            PlayerActionConfirmedEvent?.Invoke(player);
        }
        #endregion
    }
}