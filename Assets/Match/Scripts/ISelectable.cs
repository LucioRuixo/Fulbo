namespace Fulbo.Match
{
    public interface ISelectable { }

    public static class ISelectableExtensions
    {
        public static MatchPlayer AsPlayer(this ISelectable selectable)
        {
            MatchPlayerBody body = selectable as MatchPlayerBody;
            return body != null ? body.Player : null;
        }

        public static Square AsSquare(this ISelectable selectable) => selectable as Square;
    }
}