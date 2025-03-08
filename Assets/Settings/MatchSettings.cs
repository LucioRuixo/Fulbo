using System.Collections.Generic;

namespace Fulbo.Settings
{
    using Match;

    public class MatchSettings
    {
        private static Dictionary<float, SkillCheck.Difficulties> shotDifficultiesByDistanceToGoal = new Dictionary<float, SkillCheck.Difficulties>()
        {
            [7f] = SkillCheck.Difficulties.VeryEasy,
            [16f] = SkillCheck.Difficulties.Easy,
            [30f] = SkillCheck.Difficulties.Medium,
            [42f] = SkillCheck.Difficulties.Hard,
            [55f] = SkillCheck.Difficulties.VeryHard,
        };

        public static SkillCheck.Difficulties GetShotDifficulty(MatchPlayer shooter)
        {
            float shotDistance = MatchUtils.DistanceToGoal(shooter.CurrentSquare, shooter.AttackedGoal);

            foreach (float distance in shotDifficultiesByDistanceToGoal.Keys)
                if (shotDistance <= distance) return shotDifficultiesByDistanceToGoal[distance];

            return SkillCheck.Difficulties.NearlyImpossible;
        }
    }
}