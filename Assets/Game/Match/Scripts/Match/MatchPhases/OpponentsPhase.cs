namespace Fulbo.Match
{
    public class OpponentsPhase : MatchPhase
    {
        public override MatchPhases Phase => MatchPhases.Opponents;

        public override void Initialize(Match match)
        {
            base.Initialize(match);

            players = RivalTeam.Players;
        }
    }
}