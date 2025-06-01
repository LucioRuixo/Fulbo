using System.Collections.Generic;

namespace Fulbo.Settings
{
    using Match;

    public class MatchSettings
    {
        private static Dictionary<float, SkillCheck.Difficulties> passDifficultiesByDistanceToGoal = new Dictionary<float, SkillCheck.Difficulties>()
        {
            [17f] = SkillCheck.Difficulties.VeryEasy,
            [30f] = SkillCheck.Difficulties.Easy,
            [50f] = SkillCheck.Difficulties.Medium,
            [65f] = SkillCheck.Difficulties.Hard,
            [80f] = SkillCheck.Difficulties.VeryHard,
        };

        private static Dictionary<float, SkillCheck.Difficulties> shotDifficultiesByDistanceToGoal = new Dictionary<float, SkillCheck.Difficulties>()
        {
            [7f] = SkillCheck.Difficulties.VeryEasy,
            [16f] = SkillCheck.Difficulties.Easy,
            [30f] = SkillCheck.Difficulties.Medium,
            [42f] = SkillCheck.Difficulties.Hard,
            [55f] = SkillCheck.Difficulties.VeryHard,
        };

        public static int MissedPassLandingDistance => 1;

        private static SkillCheck.Difficulties GetActionDifficultyByDistance(float actionDistance, Dictionary<float, SkillCheck.Difficulties> difficulties)
        {
            foreach (float distance in shotDifficultiesByDistanceToGoal.Keys)
                if (actionDistance <= distance) return shotDifficultiesByDistanceToGoal[distance];

            return SkillCheck.Difficulties.NearlyImpossible;
        }

        public static SkillCheck.Difficulties GetPassDifficulty(MatchPlayer passer, Square receptionSquare)
            => GetActionDifficultyByDistance(MatchUtils.Distance(passer.CurrentSquare, receptionSquare), passDifficultiesByDistanceToGoal);

        public static SkillCheck.Difficulties GetShotDifficulty(MatchPlayer shooter) 
            => GetActionDifficultyByDistance(MatchUtils.DistanceToGoal(shooter.CurrentSquare, shooter.AttackedGoal), shotDifficultiesByDistanceToGoal);
    }
}