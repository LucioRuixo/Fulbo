using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        private bool endPlay = false;

        private Match match;
        private MatchPlayer selectedPlayer;

        private Dictionary<Phases, List<MatchPlayer>> phasePlayers;

        private Action processActionOnPhaseEnd;

        private float WaitInterval => 1f / speed;
        private HumanPlayer Human => match.Human;

        private event Action<MatchPlayer> PlayerActionConfirmedEvent;
        public event Action<Phases> PhaseEndedEvent;
        public event Action<int> TurnEndEvent;
        public event Action PlayEndEvent;

        private void Awake()
        {
            match = GetComponent<Match>();

            match.InitialPlayerSetEvent += OnInitialPlayerSet;
            match.PlayStartEvent += OnPlayStart;
            match.PlayEndCalledEvent += OnPlayEndCalled;
            match.MatchStartEvent += OnMatchStart;
            match.MatchEndEvent += OnMatchEnd;

            phasePlayers = new Dictionary<Phases, List<MatchPlayer>>
            {
                [Phases.Teammates] = null,
                [Phases.Dribbler ] = null,
                [Phases.Opponents] = null
            };
        }

        private void OnDestroy()
        {
            match.InitialPlayerSetEvent -= OnInitialPlayerSet;
            match.PlayStartEvent -= OnPlayStart;
            match.PlayEndCalledEvent -= OnPlayEndCalled;
            match.MatchStartEvent -= OnMatchStart;
            match.MatchEndEvent -= OnMatchEnd;
        }

        private void Play()
        {
            endPlay = false;

            InitializePhasePlayers(selectedPlayer);
            StartCoroutine(Turn());
        }

        private void Stop() => endPlay = true;

        private void InitializePhasePlayers(MatchPlayer selectedPlayer)
        {
            this.selectedPlayer = selectedPlayer;

            phasePlayers[Phases.Teammates] = selectedPlayer.Team.GetPlayers(new MatchPlayer[] { selectedPlayer });
            phasePlayers[Phases.Opponents] = selectedPlayer.Rival.Players;
        }

        private IEnumerator Turn()
        {
            while (!endPlay)
            {
                turn++;

                for (int i = 0; i < phasePlayers.Count; i++)
                {
                    if (endPlay) break;

                    yield return StartCoroutine(Phase((Phases)i, phasePlayers[(Phases)i]));
                }

                TurnEndEvent?.Invoke(turn);
            }

            PlayEndEvent?.Invoke();
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

        private void OnPlayStart() => Play();

        private void OnPlayEndCalled() => Stop();

        private void OnMatchStart() => match.Ball.DribblerSetEvent += OnDribblerSet;

        private void OnMatchEnd() => match.Ball.DribblerSetEvent -= OnDribblerSet;

        private void OnDribblerSet(MatchPlayer dribbler)
        {
            if (!dribbler) return;
            if (dribbler.Side != Human.Side) return;

            if (phasePlayers[Phases.Teammates] != null)
            {
                phasePlayers[Phases.Teammates].Remove(dribbler);
                if (selectedPlayer) phasePlayers[Phases.Teammates].Add(selectedPlayer);
            }

            selectedPlayer = dribbler;
            phasePlayers[Phases.Dribbler] = new List<MatchPlayer>() { selectedPlayer };
        }

        private void OnPlayerActionConfirmed(MatchPlayer player)
        {
            player.Brain.ActionConfirmedEvent -= OnPlayerActionConfirmed;
            PlayerActionConfirmedEvent?.Invoke(player);
        }
        #endregion
    }
}