using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fulbo.Match
{
    public class Board : MonoBehaviour
    {
        private Square[,] squares2D;

        private Match match;

        private Queue<KeyValuePair<Vector2Int, PlayerID>> moveQueue;

        public float SquareSize { get; private set; }
        public Vector2Int SquareCount { get; private set; }

        public List<Square> Squares { get; private set; }

        public event Action<Square, MatchPlayer> PlayerMovedToSquareEvent;

        private void OnDestroy()
        {
            for (int y = 0; y < SquareCount.y; y++)
            {
                for (int x = 0; x < SquareCount.x; x++)
                {
                    Get(x, y).PlayerAddedEvent -= OnPlayerAddedToSquare;
                }
            }

            match.TurnManager.PhaseEndedEvent -= OnPhaseEnded;
        }

        public void Initialize(Vector2Int squareCount, Square[,] squares2D, Match match)
        {
            this.squares2D = squares2D;
            this.match = match;

            SquareSize = match.Pitch.Length / squareCount.x;
            SquareCount = squareCount;

            Squares = new List<Square>();
            Action<Square> initializeSquares = square =>
            {
                Squares.Add(square);
                square.PlayerAddedEvent += OnPlayerAddedToSquare;
            };
            ForEachSquare(initializeSquares);
            //for (int y = 0; y < squareCount.y; y++)
            //{
            //    for (int x = 0; x < squareCount.x; x++)
            //    {
            //        Get(x, y).PlayerAddedEvent += OnPlayerAddedToSquare;
            //    }
            //}

            moveQueue = new Queue<KeyValuePair<Vector2Int, PlayerID>>();

            match.TurnManager.PhaseEndedEvent += OnPhaseEnded;
        }

        private int ClampX(int x) => Mathf.Clamp(x, 0, SquareCount.x - 1);

        private int ClampY(int y) => Mathf.Clamp(y, 0, SquareCount.y - 1);

        private void ForEachSquare(Action<Square> action)
        {
            for (int y = 0; y < SquareCount.y; y++)
            {
                for (int x = 0; x < SquareCount.x; x++)
                {
                    action(Get(x, y));
                }
            }
        }

        private void DisableHighlights() => ForEachSquare(square => { if (square.Highlighted) square.SetHighlight(false); });

        private void ExecuteQueuedMoves()
        {
            while (moveQueue.Count > 0)
            {
                KeyValuePair<Vector2Int, PlayerID> move = moveQueue.Dequeue();
                Get(move.Key).AddPlayer(match.GetTeam(move.Value.Side).Players[move.Value.Index]);
            }
        }

        public bool EnqueueMove(Vector2Int squareID, PlayerID playerID)
        {
            if (IsMoveQueued(squareID, playerID)) return false;

            moveQueue.Enqueue(new KeyValuePair<Vector2Int, PlayerID>(squareID, playerID));
            return true;
        }

        public void DequeueMove(Vector2Int squareID, PlayerID playerID)
        {
            if (!IsMoveQueued(squareID, playerID)) return;

            KeyValuePair<Vector2Int, PlayerID> remove = new KeyValuePair<Vector2Int, PlayerID>(squareID, playerID);
            moveQueue = new Queue<KeyValuePair<Vector2Int, PlayerID>>(moveQueue.Where(move => !(move.Key == remove.Key && move.Value == remove.Value)));
        }

        #region Queries
        public Square Get(int x, int y) => x < 0 || x >= SquareCount.x || y < 0 || y >= SquareCount.y ? null : squares2D[x, y];

        public Square Get(Vector2Int id) => Get(id.x, id.y);

        public bool Exists(Vector2Int id) => id.x >= 0 && id.x < SquareCount.x && id.y >= 0 && id.y < SquareCount.y;

        public bool ExistsX(int x) => x >= 0 && x < SquareCount.x;

        public bool ExistsY(int y) => y >= 0 && y < SquareCount.y;

        public bool IsEmpty(Vector2Int id, Sides side)
        {
            if (!Exists(id))
            {
                Debug.LogError("Can't check if square is empty: checked square doesn't exit.");
                return false;
            }

            return (side == Sides.Home ? Get(id).HomePlayer : Get(id).AwayPlayer) == null;
        }

        public bool IsMoveQueued(Vector2Int squareID, PlayerID playerID) => moveQueue.Any(move => move.Key == squareID && move.Value.Side == playerID.Side);

        public Square[] GetAdjacentSquares(Square reference, int distance = 1)
        {
            if (distance < 1) return null;

            int startX = ClampX(reference.X - distance);
            int endX = ClampX(reference.X + distance);
            int startY = ClampY(reference.Y - distance);
            int endY = ClampY(reference.Y + distance);

            Square[] adjacentSquares = new Square[(endX - startX + 1) * (endY - startY + 1) - 1];

            int currenIndex = 0;
            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    Square square = squares2D[x, y];

                    if (square == reference) continue;

                    adjacentSquares[currenIndex] = square;
                    currenIndex++;
                }
            }

            return adjacentSquares;
        }
        #endregion

        #region Handlers
        private void OnPlayerAddedToSquare(Square square, MatchPlayer player) => PlayerMovedToSquareEvent?.Invoke(square, player);

        private void OnPhaseEnded(TurnManager.Phases phase)
        {
            DisableHighlights();
            ExecuteQueuedMoves();
        }
        #endregion
    }
}