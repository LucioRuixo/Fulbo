using System;

namespace Fulbo.Match
{
    using UI;

    public abstract class SkillCheck : RollAction<SkillCheckData, SkillCheckResult, SkillCheckPopUp>
    {
        #region Enumerators
        public enum Difficulties
        {
            VeryEasy = 5, 
            Easy = 10, 
            Medium = 15, 
            Hard = 20, 
            VeryHard = 25, 
            NearlyImpossible = 30
        }
        #endregion

        public override int RollDie => 20;

        public static event Action<SkillCheck, bool> SetDisplayedEvent;

        public SkillCheck(MatchPlayer actor, Difficulties difficulty, bool display = true) : base(actor)
        {
            Result = new SkillCheckResult();
            Data = new SkillCheckData(
                RollDie,
                ActorAttribute,
                actor.Attributes.GetModifier(ActorAttribute),
                difficulty);
            Result.Initialize(Data);

            if (display) SetDisplayed(true);
        }

        public void SetDisplayed(bool display) => SetDisplayedEvent?.Invoke(this, display);

        public static SkillCheck RollSC<SkillCheck>(SkillCheck skillCheck, SkillCheckData forcedData = null) where SkillCheck : Fulbo.Match.SkillCheck
        {
            skillCheck.Roll(forcedData);
            return skillCheck;
        }
    }
}