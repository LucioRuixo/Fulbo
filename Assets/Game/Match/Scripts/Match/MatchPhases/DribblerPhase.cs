using System.Collections.Generic;

namespace Fulbo.Match
{
    public class DribblerPhase : MatchPhase
    {
        public override MatchPhases Phase => MatchPhases.Dribbler;

        public override void Initialize(Match match)
        {
            base.Initialize(match);

            players = new List<MatchPlayer>() { Human.SelectedPlayer };
        }
        protected override bool OnStart() => players.Count > 0;

        protected override void OnDribblerSet(MatchPlayer dribbler)
        {
            if (dribbler.Side != HumanSide) return;

            players = new List<MatchPlayer>() { dribbler };
        }

        protected override void OnDribblerCleared(MatchPlayer previousDribbler) => players = new List<MatchPlayer>();
    }
}