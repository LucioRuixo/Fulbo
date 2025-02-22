using System;
using UnityEngine;

namespace Fulbo.Match
{
    using UI;

    public abstract class Brain
    {
        private Transform actions;

        protected MatchPlayer player;
        protected Board board;
        protected MPHUD hud;

        protected MPAction chosenAction;

        protected abstract bool ShowCompleteUI { get; }

        public event Action ChooseActionEvent;
        public event Action<bool> FeedResultEvent;
        public event Action<MatchPlayer> ActionChosenEvent;
        public event Action<MatchPlayer> ActionConfirmedEvent;

        public Brain(Transform actions, MatchPlayer player, Board board, MPHUD hud)
        {
            this.actions = actions;
            this.player = player;
            this.board = board;
            this.hud = hud;
        }

        public virtual void OnUsed() { }

        public virtual void OnUnused() { }

        protected abstract void ProcessChooseAction();

        protected void OnFeedResult(bool result) => FeedResultEvent?.Invoke(result);

        protected void OnActionChosen(MPAction action)
        {
            if (!action) return;

            (chosenAction = action).OnChosen(ShowCompleteUI);
            ActionChosenEvent?.Invoke(player);
        }

        protected void OnActionCanceled()
        {
            chosenAction.OnUnchosen();
            chosenAction.OnExit();
            chosenAction = null;
        }

        protected void OnActionConfirmed() => ActionConfirmedEvent?.Invoke(player);

        public void ChooseAction()
        {
            ProcessChooseAction();
            ChooseActionEvent?.Invoke();
        }

        public void ExecuteAction()
        {
            chosenAction.OnExecuted();
            chosenAction.OnExit();
        }

        public Action GetAction<Action>() where Action : MPAction => actions.GetComponent<Action>();

        public MPAction GetActionByType(MPActions type) => type switch
        {
            MPActions.Move => GetAction<MPA_Move>(),
            MPActions.Pass => GetAction<MPA_Pass>(),
            MPActions.Shoot => GetAction<MPA_Shoot>(),
            MPActions.None => null,
            _ => null,
        };

        #region Operators
        public static implicit operator bool(Brain brain) => brain != null; 
        #endregion
    }
}