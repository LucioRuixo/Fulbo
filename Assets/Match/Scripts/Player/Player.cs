using System;
using UnityEngine;

namespace Fulbo.Match
{
    [RequireComponent(typeof(Input))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        private ISelectable selected;

        private Input input;

        public Vector3 Position => mainCamera.transform.position;
        public Camera View => mainCamera;

        public event Action<MatchPlayer> PlayerSelectedEvent;

        private void Awake() => input = new Input(this);

        private void OnEnable() => input.SelectedEvent += OnSelected;

        private void OnDisable() => input.SelectedEvent -= OnSelected;

        private void Update()
        {
            input.Update();
            //if (input.State.selected != null) Debug.Log(input.State.selected);
        }

        #region Handlers
        private void OnSelected(ISelectable newSelected)
        {
            if (selected is MatchPlayerBody)
            {
                MatchPlayer player = selected.AsPlayer();

                if (newSelected is Square) newSelected.AsSquare().AddPlayer(player);

                PlayerSelectedEvent?.Invoke(player);
            }

            selected = newSelected;
        }
        #endregion
    }
}