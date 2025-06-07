namespace Fulbo.Match
{
    using Attributes;

    public class PassAttempt : SkillCheck
    {
        public override AttributeTypes ActorAttribute => AttributeTypes.Passing;

        public PassAttempt(MatchPlayer actor, Difficulties difficulty, bool display = true) : base(actor, difficulty, display) { }
    }
}