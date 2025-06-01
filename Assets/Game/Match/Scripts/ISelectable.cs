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
            Legacy_MPBody body = selectable as Legacy_MPBody;
            return body != null ? body.Player : null;
        }

        public static Square AsSquare(this ISelectable selectable) => selectable as Square;
    }
}