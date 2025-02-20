using System;
using UnityEngine;

namespace Fulbo.Match
{
    public class Square : MonoBehaviour, ISelectable
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public Vector2Int ID { get; private set; }

        public Vector3 Position => transform.position;
        public float Size => Container.SquareSize;

        public Squares Container { get; private set; }

        public MatchPlayer HomePlayer { get; private set; }
        public MatchPlayer AwayPlayer { get; private set; }

        public event Action<Square, MatchPlayer> PlayerAddedEvent;

        private void OnDestroy()
        {
            if (Container) Container.PlayerMovedToSquareEvent += OnPlayerMovedToSquare;
        }

        public void Initialize(int x, int y, Squares container)
        {
            X = x;
            Y = y;
            ID = new Vector2Int(X, Y);

            (Container = container).PlayerMovedToSquareEvent += OnPlayerMovedToSquare;
        }

        private bool IsInSquare(MatchPlayer player) => player == HomePlayer || player == AwayPlayer;

        private MatchPlayer GetPlayer(Sides side) => side == Sides.Home ? HomePlayer : AwayPlayer;

        private MatchPlayer OtherPlayer(Sides side) => side == Sides.Home ? AwayPlayer : HomePlayer;

        private Vector3 CalculateOffsetPosition(MatchPlayer player)
        {
            float offset = Size * 0.33f * 0.5f;
            return Position - player.AttackDirection * offset;
        }

        public void AddPlayer(MatchPlayer player)
        {
            if (!player) return;

            Sides side = player.Side;

            if (side == Sides.Home && HomePlayer || side == Sides.Away && AwayPlayer) return;

            if (side == Sides.Home) HomePlayer = player;
            else AwayPlayer = player;

            MatchPlayer otherPlayer = OtherPlayer(player.Side);
            if (!otherPlayer) player.Position = Position;
            else
            {
                player.Position = CalculateOffsetPosition(player);
                otherPlayer.Position = CalculateOffsetPosition(otherPlayer);
            }

            PlayerAddedEvent?.Invoke(this, player);
        }

        #region Handlers
        private void OnPlayerMovedToSquare(Square square, MatchPlayer player)
        {
            if (square == this || !IsInSquare(player)) return;

            if (player.Side == Sides.Home)
            {
                HomePlayer = null;
                if (AwayPlayer) AwayPlayer.Position = Position;
            }
            else
            {
                AwayPlayer = null;
                if (HomePlayer) HomePlayer.Position = Position;
            }
        }
        #endregion

        #region ISelectable
        public void OnSelected()
        {
            throw new NotImplementedException();
        }

        public void OnUnselected()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}