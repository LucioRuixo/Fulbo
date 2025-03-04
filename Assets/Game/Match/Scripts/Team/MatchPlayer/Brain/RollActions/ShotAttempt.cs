namespace Fulbo.Match
{
    using Attributes;

    public class ShotAttempt : SkillCheck
    {
        public override AttributeTypes ActorAttribute => AttributeTypes.Shooting;

        public ShotAttempt(MatchPlayer actor, Difficulties difficulty, bool display = true) : base(actor, difficulty, display) { }
    }
}