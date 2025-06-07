namespace Fulbo.Match
{
    using Attributes;
    
    public class Shot : DuelRoll
    {
        public override AttributeTypes ActorAttribute => AttributeTypes.Shooting;
        public override AttributeTypes ContenderAttribute => AttributeTypes.Saving;

        public Shot(MatchPlayer actor, MatchPlayer contender, bool display = true) : base(actor, contender, display) { }
    }
}