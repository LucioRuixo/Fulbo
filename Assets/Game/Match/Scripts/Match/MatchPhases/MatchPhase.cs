using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fulbo.Match
{
    public enum MatchPhases
    {
        None = 0, 
        Teammates,
        Dribbler,
        Opponents,
        LooseBall
    }

    public enum MatchPhaseResults
    {
        None, 
        OK, 
        EarlyExit
    }

    public abstract class MatchPhase : MonoBehaviour
    {
        protected List<MatchPlayer> players;

        protected Match match;

        protected TurnManager TurnManager => match.TurnManager;

        protected Ball Ball => match.Ball;
        protected HumanPlayer Human => match.Human;
        protected Sides HumanSide => match.Human.Side;
        protected Team HumanTeam => match.GetTeam(HumanSide);
        protected Team RivalTeam => HumanTeam.Rival;

        protected bool InitialActionsExecuted { get; private set; } = false;

        public abstract MatchPhases Phase { get; }

        public bool Ended { get; protected set; } = false;
        public MatchPhaseResults Result { get; protected set; } = MatchPhaseResults.None;

        private event Action<MatchPlayer> PlayerActionConfirmedEvent;
        public event Action<MatchPhases> ExecutionEndedEvent;

        private void OnDestroy()
        {
            match.Ball.DribblerSetEvent -= OnDribblerSet;
            match.Ball.DribblerClearedEvent -= OnDribblerCleared;
        }

        public virtual void Initialize(Match match)
        {
            this.match = match;

            match.Ball.DribblerSetEvent += OnDribblerSet;
            match.Ball.DribblerClearedEvent += OnDribblerCleared;
        }

        private IEnumerator PhaseCoroutine()
        {
            Result = MatchPhaseResults.None;

            if (!OnStart())
            {
                OnEnd(MatchPhaseResults.EarlyExit);
                yield break;
            }

            // Wait for all players to select an action
            Dictionary<PlayerID, bool> results = new Dictionary<PlayerID, bool>();
            PlayerActionConfirmedEvent += (player) => { if (results.ContainsKey(player.ID)) results[player.ID] = true; };

            // Ask the players to choose an action
            foreach (MatchPlayer player in players)
            {
                player.Brain.ActionConfirmedEvent += OnPlayerActionConfirmed;
                results.Add(player.ID, false);

                ChooseAction(player);
            }

            yield return new WaitUntil(() => results.Values.All(result => result));

            PlayerActionConfirmedEvent = null;
            OnActionsConfirmed();

            yield return new WaitForSeconds(TurnManager.WaitInterval);

            foreach (MatchPlayer player in players) player.Brain.ExecuteAction();

            InitialActionsExecuted = true;
            OnActionExecuted();

            ExecutionEndedEvent?.Invoke(Phase);

            yield return new WaitForSeconds(TurnManager.WaitInterval);

            OnEnd(MatchPhaseResults.OK);
        }

        protected virtual bool OnStart() => true;

        protected virtual void ChooseAction(MatchPlayer player) => player.Brain.ChooseAction();

        protected virtual void OnActionsConfirmed() { }

        protected virtual void OnActionExecuted() { }

        protected virtual void OnEnd(MatchPhaseResults result)
        {
            Result = result;
            Ended = true;
        }

        public virtual void StartPhase()
        {
            InitialActionsExecuted = Ended = false;
            StartCoroutine(PhaseCoroutine());
        }

        #region Handlers
        private void OnPlayerActionConfirmed(MatchPlayer player, MPAction action)
        {
            player.Brain.ActionConfirmedEvent -= OnPlayerActionConfirmed;
            PlayerActionConfirmedEvent?.Invoke(player);
        }

        protected virtual void OnDribblerSet(MatchPlayer dribbler) { }

        protected virtual void OnDribblerCleared(MatchPlayer previousDribbler) { }
        #endregion
    }
}