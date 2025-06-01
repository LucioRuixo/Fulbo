using System.Linq;
using UnityEngine;

namespace Fulbo.Match
{
    public class MPA_Move : MPAction
    {
        private Square[] validSquares;
        private Square targetSquare;

        public override MPActions Type => MPActions.Move;
        public override bool RequiresFeed => true;

        private void EnqueueMove(Vector2Int targetSquareID)
        {
            targetSquare = board.Get(targetSquareID);
            board.EnqueueMove(targetSquare.ID, player.ID);
        }

        public override bool Feed(ISelectable selection)
        {
            Square square = selection.AsSquare();
            if (!square || !validSquares.Contains(square)) return targetSquare;

            EnqueueMove(square.ID);

            hud.Arrow.Show();
            hud.Arrow.Point(player.Position, targetSquare.Position);

            return true;
        }

        public override void OnChosen(bool completeUI)
        {
            validSquares = player.GetValidMovementSquares();

            if (completeUI) foreach (Square square in validSquares) square.SetHighlight(true);
        }

        public override void OnUnchosen()
        {
            if (targetSquare) board.DequeueMove(targetSquare.ID, player.ID);
        }

        public override void OnExit()
        {
            foreach (Square square in validSquares) square.SetHighlight(false);
            validSquares = null;

            targetSquare = null;

            hud.Arrow.Hide();
        }
    }
}