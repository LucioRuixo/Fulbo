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

        private MatchPlayer selectedPlayer;

        private Dictionary<Phases, List<MatchPlayer>> phasePlayers;

        private float WaitInterval => 1f / speed;

        public event Action<Phases> PhaseEndedEvent;
        public event Action<int> TurnEndedEvent;

        private void InitializePhasePlayers(MatchPlayer selectedPlayer)
        {
            this.selectedPlayer = selectedPlayer;

            phasePlayers = new Dictionary<Phases, List<MatchPlayer>>
            {
                [Phases.Teammates] = selectedPlayer.Team.GetPlayers(new MatchPlayer[] { selectedPlayer }),
                [Phases.Dribbler ] = new List<MatchPlayer>() { selectedPlayer },
                [Phases.Opponents] = selectedPlayer.Rival.Players
            };
        }

        public void Play(MatchPlayer selectedPlayer)
        {
            InitializePhasePlayers(selectedPlayer);
            StartCoroutine(Turn());
        }

        public void Stop() => StopCoroutine(Turn());

        private IEnumerator Turn()
        {
            while (true)
            {
                for (int i = 0; i < phasePlayers.Count; i++) yield return StartCoroutine(Phase((Phases)i, phasePlayers[(Phases)i]));

                turn++;
                TurnEndedEvent?.Invoke(turn);
            }
        }

        private event Action<MatchPlayer> PlayerActionConfirmedEvent;
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

            Debug.Log(phase);
            PhaseEndedEvent?.Invoke(phase);

            yield return new WaitForSeconds(WaitInterval);
        }

        #region Handlers
        private void OnPlayerActionConfirmed(MatchPlayer player)
        {
            player.Brain.ActionConfirmedEvent -= OnPlayerActionConfirmed;
            PlayerActionConfirmedEvent?.Invoke(player);
        }
        #endregion
    }
}