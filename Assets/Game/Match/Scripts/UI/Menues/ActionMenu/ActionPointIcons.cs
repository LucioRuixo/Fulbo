using UnityEngine;

namespace Fulbo.Match.UI
{
    public class ActionPointIcons : MonoBehaviour
    {
        [SerializeField] private ActionPointIcon apIcon;
        [SerializeField] private ActionPointIcon focusAPIcon;

        private HumanPlayer human;
        private MatchPlayer player;

        private ActionPoints ActionPoints => player?.ActionPoints;

        private void OnDestroy()
        {
            if (human)
            {
                human.PlayerSelectedEvent -= OnPlayerSelected;
                human.PlayerUnselectedEvent -= OnPlayerUnselected;
            }
        }

        public void Initialize(HumanPlayer human)
        {
            this.human = human;
            this.human.PlayerSelectedEvent += OnPlayerSelected;
            this.human.PlayerUnselectedEvent += OnPlayerUnselected;

            UpdateIcons();
        }

        private void UpdateIcons()
        {
            if (ActionPoints == null) return;

            if (ActionPoints.BaseAPCount == 0) apIcon.Empty();
            else apIcon.Fill();

            focusAPIcon.gameObject.SetActive(ActionPoints.FocusAP);
        }

        #region Handlers
        private void OnPlayerSelected(MatchPlayer player)
        {
            UpdateIcons();

            this.player = player;
            this.player.ActionPoints.PointsUpdatedEvent += UpdateIcons;
            this.player.Focus.FullyChargeEvent += UpdateIcons;
            this.player.Focus.EmptiedEvent += UpdateIcons;
        }

        private void OnPlayerUnselected(MatchPlayer player)
        {
            this.player.ActionPoints.PointsUpdatedEvent -= UpdateIcons;
            this.player.Focus.FullyChargeEvent -= UpdateIcons;
            this.player.Focus.EmptiedEvent -= UpdateIcons;
            this.player = null;
        }
        #endregion
    }
}