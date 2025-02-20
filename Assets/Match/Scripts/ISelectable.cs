namespace Fulbo.Match
{
    public interface ISelectable
    {
        public void OnSelected();

        public void OnUnselected();
    }

    public static class ISelectableExtensions
    {
        public static MatchPlayer AsPlayer(this ISelectable selectable)
        {
            MPBody body = selectable as MPBody;
            return body != null ? body.Player : null;
        }

        public static Square AsSquare(this ISelectable selectable) => selectable as Square;
    }
}