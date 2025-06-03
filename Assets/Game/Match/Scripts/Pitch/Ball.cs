using System;
using UnityEngine;

namespace Fulbo.Match
{
    public class Ball : MonoBehaviour
    {
        private Match match;

        private Square looseSquare;

        private Vector3? TargetPosition => Dribbler ? Dribbler.BallReference : null;

        public Vector3 Position { get => transform.position; private set => transform.position = value; }
        public Square Square => Dribbler ? Dribbler.CurrentSquare : looseSquare;
        public MatchPlayer Dribbler { get; private set; }

        public bool Dribbled => Dribbler;
        public bool Loose => !Dribbled;

        public event Action<MatchPlayer> DribblerSetEvent;
        public event Action<MatchPlayer> DribblerClearedEvent;
        public event Action LooseEvent;
        public event Action<Square> MovedToSquareEvent;

        private void Update()
        {
            if (TargetPosition.HasValue) transform.position = TargetPosition.Value;
        }

        public void Initialize(Match match) => this.match = match;

        public void SetDribbler(MatchPlayer dribbler)
        {
            if (!dribbler)
            {
                ClearDribbler();
                return;
            }

            Dribbler = dribbler;
            DribblerSetEvent?.Invoke(Dribbler);
        }

        public void ClearDribbler()
        {
            if (!Dribbler) return;

            MatchPlayer previousDribbler = Dribbler;
            Dribbler = null;

            SetSquare(previousDribbler.CurrentSquare);

            DribblerClearedEvent?.Invoke(previousDribbler);
        }

        public void SetSquare(Square square)
        {
            if (Dribbler) return;

            looseSquare = square;
            Position = square.Position;

            MovedToSquareEvent?.Invoke(Square);
        }

        public void SetFreePosition(Vector2 position)
        {
            ClearDribbler();
            looseSquare = null;

            Position = position;
        }

        #region Handlers
        private void OnPlayEnd()
        {
            if (!Dribbler) return;

            Vector3 goalPosition = Dribbler.AttackedGoal.BottomCenter;
            ClearDribbler();

            Position = goalPosition;
        }
        #endregion
    }
}