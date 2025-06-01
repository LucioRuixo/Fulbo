using System;
using System.Collections.Generic;
using System.Linq;

namespace Fulbo.Match
{
    using Fulbo.Match.UI;
    using Settings;

    public class MPA_Pass : MPAction
    {
        private List<Square> validSquares;
        private Square targetSquare;

        private List<Square> teamSquares;
        private List<Square> receptionSquares;
        private Square receptionSquare;

        private MatchPlayer receiver;

        private PassAttempt passAttempt;
        private SkillCheckPopUp passAttemptPopUp;

        private bool completeUI;

        private bool ReadyToConfirm => receiver && receptionSquare;

        public override MPActions Type => MPActions.Pass;
        public override bool RequiresFeed => true;

        public static event Action<MatchPlayer, MatchPlayer, Square, RollResult> PassAttemptEvent;

        private void OnSelectReceiver()
        {
            if (completeUI)
            {
                if (receptionSquares != null) foreach (Square square in receptionSquares) square.SetHighlight(false);
                foreach (Square square in teamSquares) square.SetHighlight(true);
            }
        }

        private void SetReceiver(MatchPlayer receiver)
        {
            this.receiver = receiver;

            SetReceptionSquare(receiver.CurrentSquare);

            hud.Arrow.Show();
            hud.Arrow.Point(player.Position, targetSquare.Position);

            if (completeUI)
            {
                foreach (Square square in teamSquares) square.SetHighlight(false);
                foreach (Square square in receptionSquares) square.SetHighlight(true);
            }
        }

        private void ClearReceiver()
        {
            receptionSquares.Remove(receiver.CurrentSquare);

            receiver = null;
            receptionSquare = null;

            validSquares = teamSquares;
            OnSelectReceiver();

            passAttempt?.SetDisplayed(false);

            hud.Arrow.Hide();
        }

        private void SetReceptionSquare(Square receptionSquare)
        {
            passAttempt?.SetDisplayed(false);

            this.receptionSquare = receptionSquare;
            UpdateReceptionSquares(receptionSquare);

            passAttempt = new PassAttempt(player, MatchSettings.GetPassDifficulty(player, this.receptionSquare));
            passAttemptPopUp = passAttempt.PopUp;

            if (receptionSquare != receiver.CurrentSquare)
            {
                receiver.HUD.Arrow.Show();
                receiver.HUD.Arrow.Point(receiver.Position, receptionSquare.Position);
            }
            else receiver.HUD.Arrow.Hide();
        }

        private void ClearReceptionSquare()
        {
            receptionSquare = receiver.CurrentSquare;
            UpdateReceptionSquares(receptionSquare);

            passAttempt?.SetDisplayed(false);

            if (completeUI) foreach (Square square in receptionSquares) square.SetHighlight(true);

            receiver.HUD.Arrow.Hide();
        }

        private void UpdateReceptionSquares(Square receptionSquare)
        {
            if (!receiver) return;

            validSquares = receiver.GetValidReceptionSquares().ToList();
            receptionSquares = validSquares.Except(new Square[] { receptionSquare }).ToList();
        }

        public override bool Feed(ISelectable selection)
        {
            Square square = selection.AsSquare();
            if (!square || !validSquares.Contains(square)) return ReadyToConfirm;

            targetSquare = square;

            if (!receiver) SetReceiver(targetSquare.GetPlayer(player.Side));
            else if (targetSquare == receptionSquare)
            {
                if (targetSquare == receiver.CurrentSquare) ClearReceiver();
                else ClearReceptionSquare();
            }
            else SetReceptionSquare(targetSquare);

            return ReadyToConfirm;
        }

        public override void OnChosen(bool completeUI)
        {
            this.completeUI = completeUI;

            validSquares = teamSquares = board.Squares.Where(square => square != player.CurrentSquare && !board.IsEmpty(square.ID, player.Side)).ToList();

            OnSelectReceiver();
        }

        public override void OnConfirmed()
        {
            passAttempt = SkillCheck.RollSC(passAttempt);
            passAttemptPopUp.UpdateContent(passAttempt.Result);
        }

        public override void Execute()
        {
            if (passAttempt.Result.Succeeded && receptionSquare != receiver.CurrentSquare) board.EnqueueMove(receptionSquare.ID, receiver.ID);
            PassAttemptEvent?.Invoke(player, receiver, receptionSquare, passAttempt.Result);
        }

        public override void OnExit()
        {
            hud.Arrow.Hide();
            if (receiver) receiver.HUD.Arrow.Hide();

            if (receptionSquare) receptionSquare.SetHighlight(false);
            if (teamSquares != null) foreach (Square square in teamSquares) square.SetHighlight(false);
            if (receptionSquares != null) foreach (Square square in receptionSquares) square.SetHighlight(false);

            passAttempt?.SetDisplayed(false);

            validSquares = teamSquares = receptionSquares = null;
            targetSquare = receptionSquare = null;
            receiver = null;
        }
    }
}