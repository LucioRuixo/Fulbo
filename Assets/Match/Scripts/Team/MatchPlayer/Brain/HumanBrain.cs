using UnityEngine;

namespace Fulbo.Match
{
    using UI;

    public class HumanBrain : Brain
    {
        private Player Human => player.Human;

        public HumanBrain(Transform actions, MatchPlayer player, Squares squares, MPHUD hud) : base(actions, player, squares, hud) { }

        protected override void ProcessChooseAction()
        {
            Human.ActionChosenEvent += OnActionChosen;
            Human.ActionCanceledEvent += OnActionCanceled;
            Human.ActionConfirmedEvent += OnActionConfirmed;
        }

        #region Handlers
        private void OnActionChosen(MPActions action)
        {
            OnActionChosen(GetActionByType(action));
            chosenAction.OnChosen();

            Human.ScreenSelectionEvent += OnScreenSelection;
        }

        private void OnActionCanceled(MPActions action)
        {
            OnActionCanceled();
            Human.ScreenSelectionEvent -= OnScreenSelection;
        }

        private void OnActionConfirmed(MPActions action)
        {
            Human.ScreenSelectionEvent -= OnScreenSelection;
            OnActionConfirmed();
        }

        private void OnScreenSelection(ISelectable selected)
        {
            bool feedResult = chosenAction.Feed(selected);
            OnFeedResult(feedResult);
        }
        #endregion
    }
}