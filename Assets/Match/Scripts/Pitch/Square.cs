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

        public Squares Container { get; private set; }
        public MatchPlayer Player { get; private set; }

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

        public void AddPlayer(MatchPlayer player)
        {
            if (!player) return;

            (Player = player).Position = Position;
            PlayerAddedEvent?.Invoke(this, Player);
        }

        #region Handlers
        private void OnPlayerMovedToSquare(Square square, MatchPlayer player)
        {
            if (square == this || player != Player) return;

            Player = null;
        }
        #endregion
    }
}