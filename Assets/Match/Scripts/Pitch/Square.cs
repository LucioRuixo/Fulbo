using System;
using UnityEngine;

namespace Fulbo.Match
{
    public class Square : MonoBehaviour, ISelectable
    {
        private MeshRenderer mesh;
        private Color color;

        public bool Highlighted { get; private set; } = false;

        public int X { get; private set; }
        public int Y { get; private set; }
        public Vector2Int ID { get; private set; }

        public Vector3 Position => transform.position;
        public float Size => Board.SquareSize;

        public Board Board { get; private set; }

        public MatchPlayer HomePlayer { get; private set; }
        public MatchPlayer AwayPlayer { get; private set; }

        public event Action<Square, MatchPlayer> PlayerAddedEvent;

        private void OnDestroy()
        {
            if (Board) Board.PlayerMovedToSquareEvent += OnPlayerMovedToSquare;
        }

        public void Initialize(int x, int y, Board container)
        {
            color = (mesh = GetComponent<MeshRenderer>()).material.color;

            X = x;
            Y = y;
            ID = new Vector2Int(X, Y);

            (Board = container).PlayerMovedToSquareEvent += OnPlayerMovedToSquare;
        }

        private bool IsInSquare(MatchPlayer player) => player == HomePlayer || player == AwayPlayer;

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

            MatchPlayer otherPlayer = GetOtherPlayer(player.Side);
            if (!otherPlayer) player.Position = Position;
            else
            {
                player.Position = CalculateOffsetPosition(player);
                otherPlayer.Position = CalculateOffsetPosition(otherPlayer);
            }

            PlayerAddedEvent?.Invoke(this, player);
        }

        public MatchPlayer GetPlayer(Sides side) => side == Sides.Home ? HomePlayer : AwayPlayer;

        public MatchPlayer GetOtherPlayer(Sides side) => side == Sides.Home ? AwayPlayer : HomePlayer;

        public void SetHighlight(bool highlight)
        {
            mesh.material.color = highlight ? color + Color.red : color;
            Highlighted = highlight;
        }

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
    }
}