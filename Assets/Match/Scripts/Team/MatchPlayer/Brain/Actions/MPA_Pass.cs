using System;
using System.Linq;

namespace Fulbo.Match
{
    public class MPA_Pass : MPAction
    {
        private Square[] validSquares;
        private Square targetSquare;

        public override bool RequiresFeed => true;

        public event Action<MatchPlayer> PassEvent;

        public override bool Feed(ISelectable selection)
        {
            Square square = selection.AsSquare();
            if (!square || !validSquares.Contains(square)) return false;

            targetSquare = square;

            hud.Arrow.Show();
            hud.Arrow.Point(player.Position, targetSquare.Position);

            return true;
        }

        public override void OnChosen(bool completeUI)
        {
            validSquares = board.Squares.Where(square => square != player.CurrentSquare && !board.IsEmpty(square.ID, player.Side)).ToArray();

            if (completeUI) foreach (Square square in validSquares) square.SetHighlight(true);
        }

        public override void OnExecuted() => PassEvent?.Invoke(targetSquare.GetPlayer(player.Side));

        public override void OnExit()
        {
            foreach (Square square in validSquares) square.SetHighlight(false);
            validSquares = null;

            targetSquare = null;

            hud.Arrow.Hide();
        }
    }
}