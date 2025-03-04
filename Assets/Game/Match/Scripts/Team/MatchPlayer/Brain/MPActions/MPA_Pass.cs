using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fulbo.Match
{
    public class MPA_Pass : MPAction
    {
        private List<Square> validSquares;
        private Square targetSquare;

        private List<Square> teamSquares;
        private List<Square> receptionSquares;
        private Square receptionSquare;

        private MatchPlayer receiver;

        private bool completeUI;

        private bool ReadyToConfirm => receiver && receptionSquare;

        public override bool RequiresFeed => true;

        public event Action<MatchPlayer, MatchPlayer, RollResult> PassEvent;

        private void OnSelectReceiver()
        {
            if (completeUI) foreach (Square square in teamSquares) square.SetHighlight(true);
        }

        private void SetReceiver(MatchPlayer receiver)
        {
            this.receiver = receiver;
            receptionSquare = receiver.CurrentSquare;

            receptionSquares.Add(receiver.CurrentSquare);
            validSquares = receptionSquares;

            hud.Arrow.Show();
            hud.Arrow.Point(player.Position, targetSquare.Position);

            if (completeUI) foreach (Square square in teamSquares) square.SetHighlight(false);
        }

        private void ClearReceiver()
        {
            receptionSquares.Remove(receiver.CurrentSquare);

            receiver = null;
            receptionSquare = null;

            validSquares = teamSquares;
            OnSelectReceiver();

            hud.Arrow.Hide();
        }

        private void SetReceptionSquare(Square receptionSquare)
        {
            this.receptionSquare = receptionSquare;

            receiver.HUD.Arrow.Show();
            receiver.HUD.Arrow.Point(receiver.Position, receptionSquare.Position);
        }

        private void ClearReceptionSquare()
        {
            receptionSquare = null;
            receiver.HUD.Arrow.Hide();
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
            receptionSquares = board.Squares.Except(teamSquares).ToList();

            OnSelectReceiver();
        }

        public override void Execute()
        {
            if (receptionSquare != receiver.CurrentSquare) board.EnqueueMove(receptionSquare.ID, receiver.ID);
            PassEvent?.Invoke(player, receiver, new RollResult());
        }

        public override void OnExit()
        {
            foreach (Square square in teamSquares) square.SetHighlight(false);
            receiver.HUD.Arrow.Hide();

            validSquares = null;
            targetSquare = null;

            teamSquares = receptionSquares = null;
            receptionSquare = null;

            receiver = null;

            hud.Arrow.Hide();
        }
    }
}