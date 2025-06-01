using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Fulbo.Match
{
    [RequireComponent(typeof(Match))]
    public class TurnManager : MonoBehaviour
    {
        [SerializeField] private Transform phaseContainer;

        [Space]

        [SerializeField] private float speed;

        private int turn = 0;
        private bool endPlay = false;

        private Match match;
        private MatchPlayer selectedPlayer;

        private MatchPhase[] phases;

        private bool PlayerActionsLeftInQueue => match.AllPlayers.Count(player => player.Brain.HasActionsInQueue) > 0;
        private MatchPlayer[] PlayersPendingActionExecution => match.AllPlayers.Where(player => player.Brain.HasActionsInQueue).ToArray();

        private HumanPlayer Human => match.Human;

        public MatchPhases CurrentPhase { get; private set; } = MatchPhases.None;
        public float WaitInterval => 1f / speed;

        public event Action<MatchPhases> PhaseEndedEvent;
        public event Action<int> TurnEndEvent;
        public event Action PlayEndEvent;

        private void Awake()
        {
            match = GetComponent<Match>();

            match.InitialPlayerSetEvent += OnInitialPlayerSet;
            match.PlayStartEvent += OnPlayStart;
            match.PlayEndCalledEvent += OnPlayEndCalled;
        }

        private void OnDestroy()
        {
            foreach (MatchPhase phase in phases) phase.ExecutionEndedEvent -= OnPhaseExecutionEnded;

            match.InitialPlayerSetEvent -= OnInitialPlayerSet;
            match.PlayStartEvent -= OnPlayStart;
            match.PlayEndCalledEvent -= OnPlayEndCalled;
        }

        private void Play()
        {
            endPlay = false;

            InitializePhases(selectedPlayer);
            CurrentPhase = MatchPhases.None;

            StartCoroutine(Turn());
        }

        private void Stop()
        {
            CurrentPhase = MatchPhases.None;
            endPlay = true;
        }

        private void InitializePhases(MatchPlayer selectedPlayer)
        {
            this.selectedPlayer = selectedPlayer;

            phases = new MatchPhase[]
            {
                phaseContainer.AddComponent<TeammatesPhase>(), 
                phaseContainer.AddComponent<DribblerPhase>(), 
                phaseContainer.AddComponent<LooseBallPhase>(), 
                phaseContainer.AddComponent<OpponentsPhase>()
            };

            foreach (MatchPhase phase in phases)
            {
                phase.Initialize(match);
                phase.ExecutionEndedEvent += OnPhaseExecutionEnded;
            }
        }

        private IEnumerator Turn()
        {
            while (!endPlay)
            {
                turn++;

                for (int i = 0; i < phases.Length; i++)
                {
                    if (endPlay) break;

                    MatchPhase phase = phases[i];

                    if (phase.Phase == MatchPhases.LooseBall && !match.Ball.Loose) continue;

                    CurrentPhase = phase.Phase;
                    phase.StartPhase();

                    yield return new WaitUntil(() => phase.Ended);
                }

                TurnEndEvent?.Invoke(turn);
            }

            PlayEndEvent?.Invoke();
        }

        //private IEnumerator Phase(MatchPhases phase, List<MatchPlayer> players)
        //{
        //    // Wait for all players to select an action
        //    Dictionary<PlayerID, bool> results = new Dictionary<PlayerID, bool>();
        //    PlayerActionConfirmedEvent += (player) => { if (results.ContainsKey(player.ID)) results[player.ID] = true; };

        //    // Ask the players to choose an action
        //    foreach (MatchPlayer player in players)
        //    {
        //        player.Brain.ActionConfirmedEvent += OnPlayerActionConfirmed;
        //        results.Add(player.ID, false);

        //        player.Brain.ChooseAction();
        //    }

        //    yield return new WaitUntil(() => results.Values.All(result => result));

        //    PlayerActionConfirmedEvent = null;

        //    yield return new WaitForSeconds(WaitInterval);

        //    do
        //    {
        //        foreach (MatchPlayer player in PlayersPendingActionExecution) player.Brain.ExecuteAction();

        //        PhaseEndedEvent?.Invoke(phase);

        //        yield return new WaitForSeconds(WaitInterval);
        //    }
        //    while (PlayerActionsLeftInQueue);
        //}

        #region Handlers
        private void OnInitialPlayerSet(MatchPlayer initialPlayer) => selectedPlayer = initialPlayer;

        private void OnPlayStart() => Play();

        private void OnPhaseExecutionEnded(MatchPhases phase) => PhaseEndedEvent?.Invoke(phase);

        private void OnPlayEndCalled() => Stop();
        #endregion
    }
}