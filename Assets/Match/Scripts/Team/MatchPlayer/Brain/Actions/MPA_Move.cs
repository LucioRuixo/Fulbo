using UnityEngine;

namespace Fulbo.Match
{
    public class MPA_Move : MPAction
    {
        private Square targetSquare;

        private void EnqueueMove(Vector2Int targetSquareID)
        {
            targetSquare = squares.Get(targetSquareID);
            squares.EnqueueMove(targetSquare.ID, player.ID);
        }

        public override bool Feed(ISelectable selection)
        {
            Square square = selection.AsSquare();

            if (!square) return false;
            if (!squares.IsEmpty(square.ID, player.Side)) return false;

            EnqueueMove(square.ID);

            hud.Arrow.Show();
            hud.Arrow.Point(player.Position, targetSquare.Position);

            return true;
        }

        public override void OnUnchosen()
        {
            squares.DequeueMove(targetSquare.ID, player.ID);
            hud.Arrow.Hide();
            targetSquare = null;
        }

        public override void OnExecuted()
        {
            hud.Arrow.Hide();
            targetSquare = null;
        }
    }
}