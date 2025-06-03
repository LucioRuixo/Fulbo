using System;
using UnityEngine;

namespace Fulbo.Match
{
    using UI;

    using UnityInput = UnityEngine.Input;

    public class Input
    {
        #region Constants
        // Raycast
        // --------------------
        private const float MaxRaycastDistance = 500f;
        private const string MatchSelectableLayerName = "MatchSelectable";
        // --------------------

        // Mouse Input
        // --------------------
        private const string LeftClick = "LeftClick";
        private const string RightClick = "RightClick";
        // --------------------
        #endregion

        #region Structures
        public struct InputState
        {
            public ISelectable pointed;
            public ISelectable selected;

            public void Refresh() => pointed = null;
        }
        #endregion

        #region Enumerators
        public enum Buttons
        {
            None = -1,
            Confirm,
            Cancel,
            COUNT
        }
        #endregion

        private HumanPlayer human;

        private InputState state;

        public event Action<ISelectable> SelectedEvent;

        public Input(HumanPlayer human) => this.human = human;

        public void Update()
        {
            state.Refresh();

            PollMouseInput();
            PollKeyboardInput();
        }

        #region Mouse Input
        private void PollMouseInput()
        {
            if (MatchMenu.BlockingPointer) return;

            ThrowRaycast();
            PollLeftClickInput();
        }

        private void ThrowRaycast()
        {
            Ray ray = human.View.ScreenPointToRay(UnityInput.mousePosition);
            state.pointed =
                Physics.Raycast(ray, out RaycastHit hitInfo, MaxRaycastDistance, LayerMask.GetMask(MatchSelectableLayerName)) && hitInfo.collider.TryGetComponent(out ISelectable pointed) ?
                pointed : null;
        }

        private void PollLeftClickInput()
        {
            if (UnityInput.GetButtonUp(LeftClick))
            {
                state.selected = state.pointed;
                if (state.selected != null) SelectedEvent?.Invoke(state.selected);
            }
        }
        #endregion

        #region Keyboard Input
        public event Action<int> NumberPressedEvent;
        public event Action<Buttons> ButtonPressedEvent;

        private void PollKeyboardInput()
        {
            PollNumberInput();
            PollButtonInput();
        }

        private void PollNumberInput()
        {
            for (int numberCode = (int)KeyCode.Alpha1; numberCode <= (int)KeyCode.Alpha9; numberCode++)
            {
                if (UnityInput.GetKeyDown((KeyCode)numberCode))
                {
                    int number = numberCode - (int)KeyCode.Alpha0;
                    NumberPressedEvent?.Invoke(number);
                    return;
                }
            }
        }

        private void PollButtonInput()
        {
            for (int i = 0; i < (int)Buttons.COUNT; i++)
            {
                Buttons button = (Buttons)i;
                if (UnityInput.GetButtonDown(button.ToString()))
                {
                    ButtonPressedEvent?.Invoke(button);
                    return;
                }
            }
        }
        #endregion
    }
}