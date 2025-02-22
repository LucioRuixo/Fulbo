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

        private MPActions chosenAction = MPActions.None;

        public event Action<MPActions> ActionChosenEvent;
        public event Action<MPActions> ActionCanceledEvent;
        public event Action<MPActions> ActionConfirmedEvent;

        public override void Enable()
        {
            chosenAction = MPActions.None;

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
            chosenAction = (MPActions)action;

            actionButtonsContainer.SetActive(false);
            navigationButtonsContainer.gameObject.SetActive(true);

            ActionChosenEvent?.Invoke(chosenAction);
        }

        public void OnCancel()
        {
            chosenAction = MPActions.None;

            navigationButtonsContainer.gameObject.SetActive(false);
            actionButtonsContainer.SetActive(true);

            ActionCanceledEvent?.Invoke(chosenAction);
        }

        public void OnConfirm() => ActionConfirmedEvent?.Invoke(chosenAction);

        public void SetConfirmButtonEnabled(bool enabled) => confirmButton.interactable = enabled;

        #region Handlers
        private void OnNumberPressed(int number)
        {
            if (chosenAction != MPActions.None) return;

            OnAction(number - 1);
        }

        private void OnButtonPressed(Buttons button)
        {
            if (chosenAction == MPActions.None) return;

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