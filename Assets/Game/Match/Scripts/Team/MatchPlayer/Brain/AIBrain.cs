using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Match
{
    using UI;

    public class AIBrain : Brain
    {
        // TESTING
        // --------------------
        private KeyValuePair<bool, int>[] moves = new KeyValuePair<bool, int>[]
        {
            new KeyValuePair<bool, int> ( true, -1),
            new KeyValuePair<bool, int> ( true,  1),
            new KeyValuePair<bool, int> (false, -1),
            new KeyValuePair<bool, int> (false,  1),
        };
        // --------------------

        protected override bool ShowCompleteUI => false;

        public AIBrain(Transform actions, MatchPlayer player, Board board, MPHUD hud) : base(actions, player, board, hud) { }

        protected override void ProcessChooseAction()
        {
            OnActionChosen(GetAction<MPA_Move>());
            if (!player.IsGK) TrySelectRandomAdjacentSquare();
            OnActionConfirmed();
        }

        // TESTING
        // --------------------
        private void TrySelectRandomAdjacentSquare()
        {
            List<int> indeces = new List<int>();
            List<int> randomIndeces = new List<int>();
            for (int i = 0; i < moves.Length; i++) indeces.Add(i);
            for (int i = 0; i < moves.Length; i++)
            {
                int randomIndex = Random.Range(0, indeces.Count);
                randomIndeces.Add(indeces[randomIndex]);
                indeces.RemoveAt(randomIndex);
            }

            Square currentSquare = player.CurrentSquare;

            Square targetSquare = null;
            for (int i = 0; i < randomIndeces.Count; i++)
            {
                KeyValuePair<bool, int> move = moves[randomIndeces[i]];

                int triedIndex;
                if (move.Key)
                {
                    triedIndex = player.CurrentSquare.X + move.Value;
                    Vector2Int triedID = new Vector2Int(triedIndex, currentSquare.Y);
                    if (board.ExistsX(triedIndex) && board.IsEmpty(triedID, player.Side) && !board.IsMoveQueued(triedID, player.ID))
                    {
                        targetSquare = board.Get(triedID);
                        break;
                    }
                }
                else
                {
                    triedIndex = player.CurrentSquare.Y + move.Value;
                    Vector2Int triedID = new Vector2Int(currentSquare.X, triedIndex);
                    if (board.ExistsY(triedIndex) && board.IsEmpty(triedID, player.Side) && !board.IsMoveQueued(triedID, player.ID))
                    {
                        targetSquare = board.Get(triedID);
                        break;
                    }
                }
            }

            if (targetSquare) GetAction<MPA_Move>().Feed(targetSquare);
        }
        // --------------------
    }
}