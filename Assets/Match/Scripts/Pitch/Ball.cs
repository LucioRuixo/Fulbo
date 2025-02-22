using System;
using UnityEngine;

namespace Fulbo.Match
{
    public class Ball : MonoBehaviour
    {
        private Match match;

        private Vector3? TargetPosition => Dribbler ? Dribbler.BallReference : null;

        public Vector3 Position { get => transform.position; private set => transform.position = value; }
        public MatchPlayer Dribbler { get; private set; }

        public event Action<MatchPlayer> DribblerSetEvent;
        public event Action DribblerClearedEvent;

        private void Update()
        {
            if (TargetPosition.HasValue) transform.position = TargetPosition.Value;
        }

        private void OnDestroy()
        {
            match.InitialPlayerSetEvent -= SetDribbler;
            match.TurnManager.PassEvent -= OnPass;
            match.TurnManager.ShotEvent -= OnShot;
        }

        public void Initialize(Match match)
        {
            this.match = match;
            match.InitialPlayerSetEvent += SetDribbler;
            match.TurnManager.PassEvent += OnPass;
            match.TurnManager.ShotEvent += OnShot;
        }

        private void SetDribbler(MatchPlayer dribbler)
        {
            if (!dribbler)
            {
                ClearDribbler();
                return;
            }

            Dribbler = dribbler;
            DribblerSetEvent?.Invoke(Dribbler);
        }

        private void ClearDribbler()
        {
            Dribbler = null;

            DribblerClearedEvent?.Invoke();
        }

        #region Handlers
        private void OnPass(MatchPlayer receiver)
        {
            if (!receiver) return;

            SetDribbler(receiver);
        }

        private void OnShot()
        {
            if (!Dribbler) return;

            Vector3 goalPosition = Dribbler.AttackedGoal.BottomCenter;
            ClearDribbler();

            Position = goalPosition;
        }
        #endregion
    }
}