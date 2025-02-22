using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fulbo.Match.UI
{
    using static Input;

    public class ActionMenu : Menu
    {
        [SerializeField] private GameObject actionButtonsContainer;
        [SerializeField] private GameObject navigationButtonsContainer;

        [Space]

        [SerializeField] private Button confirmButton;

        private MPActions selectedAction = MPActions.None;

        public event Action<MPActions> ActionChosenEvent;
        public event Action<MPActions> ActionCanceledEvent;
        public event Action<MPActions> ActionConfirmedEvent;

        public override void Enable()
        {
            actionButtonsContainer.SetActive(true);
            navigationButtonsContainer.SetActive(false);

            player.Input.NumberPressedEvent += OnNumberPressed;
            player.Input.ButtonPressedEvent += OnButtonPressed;

            base.Enable();
        }

        public override void Disable()
        {
            player.Input.NumberPressedEvent -= OnNumberPressed;
            player.Input.ButtonPressedEvent -= OnButtonPressed;

            base.Disable();
        }

        public void OnAction(int action)
        {
            selectedAction = (MPActions)action;

            actionButtonsContainer.SetActive(false);
            navigationButtonsContainer.gameObject.SetActive(true);

            ActionChosenEvent?.Invoke(selectedAction);
        }

        public void OnCancel()
        {
            selectedAction = MPActions.None;

            navigationButtonsContainer.gameObject.SetActive(false);
            actionButtonsContainer.SetActive(true);

            ActionCanceledEvent?.Invoke(selectedAction);
        }

        public void OnConfirm() => ActionConfirmedEvent?.Invoke(selectedAction);

        public void SetConfirmButtonEnabled(bool enabled) => confirmButton.interactable = enabled;

        #region Handlers
        private void OnNumberPressed(int number) => OnAction(number - 1);

        private void OnButtonPressed(Buttons button)
        {
            switch (button)
            {
                case Buttons.Confirm:
                    if (confirmButton.interactable) OnConfirm();
                    break;

                case Buttons.Cancel:
                    OnCancel();
                    break;
            }
        }
        #endregion
    }
}