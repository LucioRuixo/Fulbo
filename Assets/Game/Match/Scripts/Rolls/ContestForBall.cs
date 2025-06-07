namespace Fulbo.Match
{
    using Attributes;

    public class ContestForBall : DuelRoll
    {
        public override AttributeTypes ActorAttribute => AttributeTypes.Speed | AttributeTypes.Strength;
        public override AttributeTypes ContenderAttribute => AttributeTypes.Speed | AttributeTypes.Strength;

        public ContestForBall(MatchPlayer actor, MatchPlayer contender, bool display = true) : base(actor, contender, display) { }
    }
}