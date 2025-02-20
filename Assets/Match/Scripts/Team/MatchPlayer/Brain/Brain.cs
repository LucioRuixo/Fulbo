using System;
using UnityEngine;

namespace Fulbo.Match
{
    using UI;

    public abstract class Brain
    {
        private Transform actions;

        protected MatchPlayer player;
        protected Squares squares;
        protected MPHUD hud;

        protected MPAction chosenAction;

        public event Action ChooseActionEvent;
        public event Action<bool> FeedResultEvent;
        public event Action<MatchPlayer> ActionChosenEvent;
        public event Action<MatchPlayer> ActionConfirmedEvent;

        public Brain(Transform actions, MatchPlayer player, Squares squares, MPHUD hud)
        {
            this.actions = actions;
            this.player = player;
            this.squares = squares;
            this.hud = hud;
        }

        public virtual void OnUsed() { }

        public virtual void OnUnused() { }

        protected Action GetAction<Action>() where Action : MPAction => actions.GetComponent<Action>();

        protected MPAction GetActionByType(MPActions type) => type switch
        {
            MPActions.Move => GetAction<MPA_Move>(),
            MPActions.None => null,
            _ => null,
        };

        protected abstract void ProcessChooseAction();

        protected void OnFeedResult(bool result) => FeedResultEvent?.Invoke(result);

        protected void OnActionChosen(MPAction action)
        {
            if (!action) return;

            (chosenAction = action).OnChosen();
            ActionChosenEvent?.Invoke(player);
        }

        protected void OnActionCanceled()
        {
            chosenAction.OnUnchosen();
            chosenAction = null;
        }

        protected void OnActionConfirmed() => ActionConfirmedEvent?.Invoke(player);

        protected void OnActionExecuted()
        {

        }

        public void ChooseAction()
        {
            ProcessChooseAction();
            ChooseActionEvent?.Invoke();
        }

        public void ExecuteAction()
        {
            chosenAction.OnExecuted();
        }

        #region Operators
        public static implicit operator bool(Brain brain) => brain != null; 
        #endregion
    }
}