using UnityEngine;

namespace Fulbo.Match
{
    using UI;

    public class HumanBrain : Brain
    {
        protected override bool ShowCompleteUI => true;

        private HumanPlayer Human => player.Human;

        public HumanBrain(Transform actions, MatchPlayer player, Board board, MPHUD hud) : base(actions, player, board, hud) { }

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
            Human.ScreenSelectionEvent += OnScreenSelection;
        }

        private void OnActionCanceled(MPActions action)
        {
            OnActionCanceled();
            Human.ScreenSelectionEvent -= OnScreenSelection;
        }

        private void OnActionConfirmed(MPActions action)
        {
            Human.ActionChosenEvent -= OnActionChosen;
            Human.ActionCanceledEvent -= OnActionCanceled;
            Human.ActionConfirmedEvent -= OnActionConfirmed;

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