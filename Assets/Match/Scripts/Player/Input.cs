using System;
using UnityEngine;

namespace Fulbo.Match
{
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

        private Player player;

        private InputState state;
        public InputState State => state;

        public event Action<ISelectable> SelectedEvent;

        public Input(Player player) => this.player = player;

        public void Update()
        {
            state.Refresh();

            ThrowRaycast();
            PollMouseInput();
        }

        #region Raycast
        private void ThrowRaycast()
        {
            Ray ray = player.View.ScreenPointToRay(UnityInput.mousePosition);
            state.pointed = 
                Physics.Raycast(ray, out RaycastHit hitInfo, MaxRaycastDistance, LayerMask.GetMask(MatchSelectableLayerName)) && hitInfo.collider.TryGetComponent(out ISelectable pointed) ? 
                pointed : null;
        }
        #endregion

        #region Mouse Input
        private void PollMouseInput()
        {
            PollLeftClickInput();
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
    }
}