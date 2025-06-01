using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Match
{
    using UI;

    public class LooseBallPhase : MatchPhase
    {
        private MatchPlayer player;
        private MatchPlayer opponent;

        private Square[] playerPathToBall;
        private Square[] opponentPathToBall;

        private ContestForBall contest;
        private DuelPopUp contestPopUp;

        private Board Board => match.Pitch.Board;

        public override MatchPhases Phase => MatchPhases.LooseBall;

        protected override bool OnStart()
        {
            players = new List<MatchPlayer>();

            player = HumanTeam.GetRandomClosestPlayerToSquare(match.Ball.Square, new MatchPlayer[] { HumanTeam.GK });
            playerPathToBall = GetPathToBall(player);

            opponent = RivalTeam.GetRandomClosestPlayerToSquare(match.Ball.Square, new MatchPlayer[] { RivalTeam.GK });
            opponentPathToBall = GetPathToBall(opponent);

            if (playerPathToBall.Length <= 2) players.Add(player);
            if (opponentPathToBall.Length <= 2) players.Add(opponent);

            return players.Count > 0;
        }

        private Square[] GetPathToBall(MatchPlayer player) 
            => player.CurrentSquare == Ball.Square ? new Square[] { player.CurrentSquare } : Board.Path(player.CurrentSquare, Ball.Square);

        protected override void ChooseAction(MatchPlayer player)
        {
            Square square;

            Square[] pathToBall = player.Side == HumanSide ? playerPathToBall : opponentPathToBall;
            square = pathToBall[Mathf.Clamp(MatchPlayer.MovementDistance, 0, pathToBall.Length - 1)];

            player.Brain.ForceChooseAction<MPA_Move>(square);
        }

        protected override void OnActionsConfirmed()
        {
            if (players.Count < 2) return;

            contest = new ContestForBall(player, opponent);
            contestPopUp = contest.PopUp;
        }

        protected override void OnActionExecuted()
        {
            if (contest == null) return;

            contest = DuelRoll.RollDuel(contest);
            contestPopUp.UpdateContent(contest.Result);

            Ball.SetDribbler(contest.Result.Succeeded ? player : opponent);
        }

        protected override void OnEnd(MatchPhaseResults result)
        {
            playerPathToBall = opponentPathToBall = null;
            players = new List<MatchPlayer>();

            contest?.SetDisplayed(false);
            contest = null;
            contestPopUp = null;

            base.OnEnd(result);
        }
    }
}