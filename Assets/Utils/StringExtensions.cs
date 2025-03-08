namespace Fulbo
{
    public static class StringExtensions
    {
        public static string Abbreviate(this string text) => text.Substring(0, 3).ToUpper();
    }
}