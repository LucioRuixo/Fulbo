using System.Linq;

namespace Fulbo.Match
{
    public class TeammatesPhase : MatchPhase
    {
        public override MatchPhases Phase => MatchPhases.Teammates;

        public override void Initialize(Match match)
        {
            base.Initialize(match);

            players = HumanTeam.GetPlayers(new MatchPlayer[] { Human.SelectedPlayer }).ToList();
        }

        protected override void OnDribblerSet(MatchPlayer dribbler)
        {
            if (dribbler.Side != HumanSide) return;

            players = HumanTeam.GetPlayers(new MatchPlayer[] { dribbler }).ToList();
        }

        protected override void OnDribblerCleared(MatchPlayer previousDribbler) => players = HumanTeam.Players;
    }
}