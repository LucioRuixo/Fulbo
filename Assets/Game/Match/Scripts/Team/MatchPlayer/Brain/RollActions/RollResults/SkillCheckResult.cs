using Fulbo.Attributes;

namespace Fulbo.Match
{
    using static SkillCheck;

    #region Classes
    public class SkillCheckData : RollData
    {
        public Difficulties? Difficulty { get; private set; }

        public SkillCheckData() : base() { }

        public SkillCheckData(int die, AttributeTypes attribute, int actorModifier, Difficulties? difficulty = null, int? actorRoll = null) : 
            base(die, attribute, actorModifier, difficulty.HasValue ? (int)difficulty : null, actorRoll)
            => Difficulty = difficulty;
    }
    #endregion

    public class SkillCheckResult : RollResult
    {
        public Difficulties? Difficulty { get; private set; }

        public override void Initialize(RollData data)
        {
            SkillCheckData skillCheckData = data as SkillCheckData;
            Difficulty = skillCheckData.Difficulty;

            base.Initialize(data);
        }
    }
}