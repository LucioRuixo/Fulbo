namespace Fulbo.Match
{
    public class MatchUtils
    {
        public static float DistanceToGoal(Square square, Goal goal) => (goal.transform.position - square.Position).Horizontal().magnitude.Rounded(1);
    }
}