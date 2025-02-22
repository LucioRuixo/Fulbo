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

        private void OnDestroy() => match.InitialPlayerSetEvent -= SetDribbler;

        public void Initialize(Match match) => (this.match = match).InitialPlayerSetEvent += SetDribbler;

        private void SetDribbler(MatchPlayer dribbler)
        {
            if (!dribbler)
            {
                ClearDribbler();
                return;
            }

            Dribbler = dribbler;
            Dribbler.Brain.GetAction<MPA_Pass>().PassEvent += OnPass;
            Dribbler.Brain.GetAction<MPA_Shoot>().ShotEvent += OnShot;

            DribblerSetEvent?.Invoke(Dribbler);
        }

        private void ClearDribbler()
        {
            Dribbler.Brain.GetAction<MPA_Pass>().PassEvent -= OnPass;
            Dribbler.Brain.GetAction<MPA_Shoot>().ShotEvent -= OnShot;
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