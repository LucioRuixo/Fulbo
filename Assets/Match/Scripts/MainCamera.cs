using UnityEngine;

namespace Fulbo.Match
{
    [RequireComponent(typeof(Camera))]
    public class MainCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 referenceDirection;
        [SerializeField] private Vector3 rotation;

        [Space]

        [SerializeField] private Player player;

        private MatchPlayer target;

        private void Awake() => player.PlayerSelectedEvent += OnPlayerSelected;

        private void LateUpdate() => UpdatePosition();

        private void OnDestroy() => player.PlayerSelectedEvent -= OnPlayerSelected;

        private void UpdatePosition()
        {
            if (!target) return;

            Vector3 toGoalDirection = (target.AttackedGoal.BottomCenter - target.Position).Horizontal().normalized;

            Quaternion toGoalRotation = Quaternion.FromToRotation(referenceDirection.normalized, toGoalDirection);
            transform.position = target.Position + (toGoalRotation * offset);

            float toGoalAngle = Vector3.SignedAngle(referenceDirection.normalized, toGoalDirection, Vector3.up);
            transform.rotation = Quaternion.AngleAxis(toGoalAngle, Vector3.up) * Quaternion.Euler(rotation);
        }

        #region Handlers
        private void OnPlayerSelected(MatchPlayer player) => target = player;
        #endregion
    }
}