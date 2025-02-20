using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fulbo.Match
{
    public class Squares : MonoBehaviour
    {
        private Square[,] squares;

        private Match match;

        private Queue<KeyValuePair<Vector2Int, PlayerID>> moveQueue;

        public float SquareSize { get; private set; }
        public Vector2Int SquareCount { get; private set; }

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

        public void Initialize(Vector2Int squareCount, Square[,] squares, Match match)
        {
            this.squares = squares;
            this.match = match;

            SquareSize = match.Pitch.Length / squareCount.x;
            SquareCount = squareCount;

            for (int y = 0; y < squareCount.y; y++)
            {
                for (int x = 0; x < squareCount.x; x++)
                {
                    Get(x, y).PlayerAddedEvent += OnPlayerAddedToSquare;
                }
            }

            moveQueue = new Queue<KeyValuePair<Vector2Int, PlayerID>>();

            match.TurnManager.PhaseEndedEvent += OnPhaseEnded;
        }

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

        public Square Get(int x, int y) => x < 0 || x >= SquareCount.x || y < 0 || y >= SquareCount.y ? null : squares[x, y];

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

        #region Handlers
        private void OnPlayerAddedToSquare(Square square, MatchPlayer player) => PlayerMovedToSquareEvent?.Invoke(square, player);

        private void OnPhaseEnded(TurnManager.Phases phase) => ExecuteQueuedMoves();

        //private void OnTurnEnded(int turn) => ExecuteQueuedMoves();
        #endregion
    }
}