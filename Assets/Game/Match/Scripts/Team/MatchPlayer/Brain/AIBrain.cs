using System.Collections.Generic;
using System.Linq;
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

        public AIBrain(Transform actions, MatchPlayer player, Match match, MPHUD hud) : base(actions, player, match, hud) { }

        protected override void ProcessChooseAction()
        {
            OnActionChosen(GetAction<MPA_Move>());

            if (!player.IsGK) TrySelectMoveSquare();

            OnActionConfirmed();
        }

        // TESTING
        // --------------------
        private void TrySelectMoveSquare()
        {
            if (player.Team.GetClosestPlayersToSquare(Ball.Square).Contains(player))
            {
                GetAction<MPA_Move>().Feed(Board.NextSquareTo(player.CurrentSquare, Ball.Square));
                return;
            }

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
                    if (Board.ExistsX(triedIndex) && Board.IsEmpty(triedID, player.Side) && !Board.IsMoveQueued(triedID, player.ID))
                    {
                        targetSquare = Board.Get(triedID);
                        break;
                    }
                }
                else
                {
                    triedIndex = player.CurrentSquare.Y + move.Value;
                    Vector2Int triedID = new Vector2Int(currentSquare.X, triedIndex);
                    if (Board.ExistsY(triedIndex) && Board.IsEmpty(triedID, player.Side) && !Board.IsMoveQueued(triedID, player.ID))
                    {
                        targetSquare = Board.Get(triedID);
                        break;
                    }
                }
            }

            if (targetSquare) GetAction<MPA_Move>().Feed(targetSquare);
        }
        // --------------------
    }
}