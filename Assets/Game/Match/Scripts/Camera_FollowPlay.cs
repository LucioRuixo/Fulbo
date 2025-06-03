using System.Collections;
using UnityEngine;

namespace Fulbo.Match
{
    [RequireComponent(typeof(Camera))]
    public class Camera_FollowPlay : MonoBehaviour
    {
        [SerializeField] private Match match;

        [Space]

        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 referenceDirection;
        [SerializeField] private Vector3 rotation;

        [Space]

        [SerializeField] private float baseDistance;
        [SerializeField] private float baseDuration;
        [SerializeField] private AnimationCurve movementSpeed;

        private bool initialized = false;

        private bool moving = false;

        private Transform target;

        private Vector3 initialPosition;
        private float initialAngle;

        private Vector3? directionToGoal;
        private Vector3? targetPosition;
        private float? targetAngle;

        private Vector3 DirectionToGoal => directionToGoal ??= (match.GetTeam(match.LastPossessingSide).AttackedGoal.BottomCenter - target.position).HorizontalDirection();
        private Vector3 TargetPosition => targetPosition ??= target.position + (Quaternion.FromToRotation(referenceDirection.normalized, DirectionToGoal) * offset);
        private float TargetAngle => targetAngle ??= Vector3.SignedAngle(referenceDirection.normalized, DirectionToGoal, Vector3.up);

        private Ball Ball => match.Ball;

        private void LateUpdate()
        {
            if (!initialized)
            {
                if (Ball) Initialize();
                else return;
            }

            if (!moving && transform.position != TargetPosition) StartCoroutine(MoveToTargetCoroutine());

            directionToGoal = targetPosition = null;
            targetAngle = null;
        }

        private void OnDestroy()
        {
            Ball.DribblerSetEvent -= OnDribblerSet;
            Ball.MovedToSquareEvent -= OnBallMovedToSquare;
        }

        private void Initialize()
        {
            target = Ball.Dribbler ? Ball.Dribbler.transform : Ball.transform;

            transform.position = TargetPosition;
            float toGoalAngle = Vector3.SignedAngle(referenceDirection.normalized, DirectionToGoal, Vector3.up);
            transform.rotation = Quaternion.AngleAxis(toGoalAngle, Vector3.up) * Quaternion.Euler(rotation);

            UpdateInitialValues();

            Ball.DribblerSetEvent += OnDribblerSet;
            Ball.MovedToSquareEvent += OnBallMovedToSquare;

            initialized = true;
        }

        private float GetCurrentAngle() => Vector3.SignedAngle(referenceDirection.normalized, transform.forward.HorizontalDirection(), Vector3.up);

        private void UpdateInitialValues()
        {
            initialPosition = transform.position;
            initialAngle = GetCurrentAngle();
        }

        private void SetTarget(Transform target)
        {
            if (target == this.target) return;

            this.target = target;

            StopMovement();
            if (initialPosition != TargetPosition) StartCoroutine(MoveToTargetCoroutine());
        }

        private void StopMovement()
        {
            StopAllCoroutines();
            moving = false;
        }

        #region Coroutines
        private IEnumerator MoveToTargetCoroutine()
        {
            UpdateInitialValues();

            float distance = initialPosition.DistanceTo(TargetPosition);
            float duration = baseDuration * Mathf.Max(distance / baseDistance, 1f);

            moving = true;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = movementSpeed.Evaluate(elapsed / duration);
                transform.position = Vector3.Lerp(initialPosition, TargetPosition, t);
                transform.rotation = Quaternion.AngleAxis(Mathf.Lerp(initialAngle, TargetAngle, t), Vector3.up) * Quaternion.Euler(rotation);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = TargetPosition;
            moving = false;
        }
        #endregion

        #region Handlers
        private void OnDribblerSet(MatchPlayer dribbler) => SetTarget(dribbler.transform);

        private void OnBallMovedToSquare(Square square)
        {
            if (Ball.Dribbler) return;

            SetTarget(Ball.transform);
        }
        #endregion
    }
}