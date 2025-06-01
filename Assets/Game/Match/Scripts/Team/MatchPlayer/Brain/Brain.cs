using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Match
{
    using UI;

    public abstract class Brain
    {
        private Transform actions;

        protected MatchPlayer player;
        protected Match match;
        protected MPHUD hud;

        protected MPAction targetAction;

        protected Ball Ball => match.Ball;
        protected Board Board => match.Pitch.Board;

        protected Queue<MPActions> actionQueue = new Queue<MPActions>();
        public bool HasActionsInQueue => actionQueue.Count > 0;

        protected abstract bool ShowCompleteUI { get; }

        public event Action ChooseActionEvent;
        public event Action<bool> FeedResultEvent;
        public event Action<MatchPlayer> ActionChosenEvent;
        public event Action<MatchPlayer> ActionConfirmedEvent;

        public Brain(Transform actions, MatchPlayer player, Match match, MPHUD hud)
        {
            this.actions = actions;
            this.player = player;
            this.match = match;
            this.hud = hud;
        }

        public virtual void OnUsed() { }

        public virtual void OnUnused() { }

        protected abstract void ProcessChooseAction();

        protected void OnFeedResult(bool result) => FeedResultEvent?.Invoke(result);

        protected void OnActionChosen(MPAction action)
        {
            if (!action) return;

            (targetAction = action).OnChosen(ShowCompleteUI);
            ActionChosenEvent?.Invoke(player);
        }

        protected void OnActionCanceled()
        {
            targetAction.OnUnchosen();
            targetAction.OnExit();
            targetAction = null;
        }

        protected void OnActionConfirmed()
        {
            actionQueue.Enqueue(targetAction.Type);
            targetAction.OnConfirmed();
            targetAction = null;

            ActionConfirmedEvent?.Invoke(player);
        }

        public void ChooseAction()
        {
            ProcessChooseAction();
            ChooseActionEvent?.Invoke();
        }

        public void ExecuteAction()
        {
            if (actionQueue.Count == 0) return;

            MPAction action = GetActionByType(actionQueue.Dequeue());
            action.Execute();
            action.OnExit();
        }

        public bool TryGetAction<Action>(out Action action) where Action : MPAction => actions.TryGetComponent(out action);

        public Action GetAction<Action>() where Action : MPAction
        {
            TryGetAction(out Action action);
            return action;
        }

        public MPAction GetActionByType(MPActions type) => type switch
        {
            MPActions.Move => GetAction<MPA_Move>(),
            MPActions.Pass => GetAction<MPA_Pass>(),
            MPActions.Shoot => GetAction<MPA_Shoot>(),
            MPActions.None => null,
            _ => null,
        };

        public void ForceChooseAction<Action>(ISelectable feed) where Action : MPAction
        {
            if (!TryGetAction(out Action action)) return;

            OnActionChosen(action);
            if (feed != null) action.Feed(feed);
            OnActionConfirmed();
        }

        #region Operators
        public static implicit operator bool(Brain brain) => brain != null; 
        #endregion
    }
}