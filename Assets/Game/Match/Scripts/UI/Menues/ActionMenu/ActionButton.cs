using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Match.UI
{
    [RequireComponent(typeof(Button))]
    public class ActionButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        [Space]

        [SerializeField] private MPActions action;

        private HumanPlayer human;

        private MatchPlayer Player => human?.SelectedPlayer;

        private void OnDestroy()
        {
            human.PlayerSelectedEvent -= OnPlayerSelected;
            human.PlayerUnselectedEvent -= OnPlayerUnselected;
            human.ActionMenuEnabledEvent -= UpdateInteractability;
        }

        public void Initialize(HumanPlayer human)
        {
            this.human = human;
            this.human.PlayerSelectedEvent += OnPlayerSelected;
            this.human.PlayerUnselectedEvent += OnPlayerUnselected;
            this.human.ActionMenuEnabledEvent += UpdateInteractability;
        }

        private void UpdateInteractability()
        {
            if (!Player)
            {
                button.interactable = false;
                return;
            }

            button.interactable = Player.Brain.GetActionByType(action).CanBeChosen;
        }

        #region Handlers
        private void OnPlayerSelected(MatchPlayer player)
        {
            UpdateInteractability();

            player.ActionPoints.PointsUpdatedEvent += OnPlayerAPUpdate;
        }

        private void OnPlayerUnselected(MatchPlayer player) => player.ActionPoints.PointsUpdatedEvent -= OnPlayerAPUpdate;

        private void OnPlayerAPUpdate() => UpdateInteractability();
        #endregion
    }
}