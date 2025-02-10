using System;
using UnityEngine;

namespace Fulbo.Match
{
    public class Squares : MonoBehaviour
    {
        private Vector2Int squareCount;
        private Square[,] squares;

        public event Action<Square, MatchPlayer> PlayerMovedToSquareEvent;

        private void OnDestroy()
        {
            for (int y = 0; y < squareCount.y; y++)
            {
                for (int x = 0; x < squareCount.x; x++)
                {
                    Get(x, y).PlayerAddedEvent -= OnPlayerAddedToSquare;
                }
            }
        }

        public void Initialize(Vector2Int squareCount, Square[,] squares)
        {
            this.squareCount = squareCount;
            this.squares = squares;

            for (int y = 0; y < squareCount.y; y++)
            {
                for (int x = 0; x < squareCount.x; x++)
                {
                    Get(x, y).PlayerAddedEvent += OnPlayerAddedToSquare;
                }
            }
        }

        public Square Get(int x, int y) => x < 0 || x >= squareCount.x || y < 0 || y >= squareCount.y ? null : squares[x, y];

        #region Handlers
        private void OnPlayerAddedToSquare(Square square, MatchPlayer player) => PlayerMovedToSquareEvent?.Invoke(square, player);
        #endregion
    }
}