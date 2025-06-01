namespace Fulbo.Match
{
    public class MatchUtils
    {
        public static float Distance(Square a, Square b) => a.Position.HorizontalDistanceTo(b.Position).Rounded(1);

        public static float DistanceToGoal(Square square, Goal goal) => square.Position.HorizontalDistanceTo(goal.transform.position).Rounded(1);
    }
}