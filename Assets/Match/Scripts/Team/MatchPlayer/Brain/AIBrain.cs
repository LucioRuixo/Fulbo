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

        public AIBrain(Transform actions, MatchPlayer player, Squares squares, MPHUD hud) : base(actions, player, squares, hud) { }

        protected override void ProcessChooseAction()
        {
            TrySelectRandomAdjacentSquare();
            OnActionChosen(GetAction<MPA_Move>());
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
                    if (squares.ExistsX(triedIndex) && squares.IsEmpty(triedID, player.Side) && !squares.IsMoveQueued(triedID, player.ID))
                    {
                        targetSquare = squares.Get(triedID);
                        break;
                    }
                }
                else
                {
                    triedIndex = player.CurrentSquare.Y + move.Value;
                    Vector2Int triedID = new Vector2Int(currentSquare.X, triedIndex);
                    if (squares.ExistsY(triedIndex) && squares.IsEmpty(triedID, player.Side) && !squares.IsMoveQueued(triedID, player.ID))
                    {
                        targetSquare = squares.Get(triedID);
                        break;
                    }
                }
            }

            if (targetSquare) GetAction<MPA_Move>().Feed(targetSquare);
        }
        // --------------------
    }
}