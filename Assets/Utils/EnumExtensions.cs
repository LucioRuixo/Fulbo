using System;
using System.Collections.Generic;
using System.Linq;

namespace Fulbo
{
    public static class EnumExtensions
    {
        public static IEnumerable<Enum> GetFlags<Enum>(this Enum value) where Enum : System.Enum => System.Enum.GetValues(value.GetType()).Cast<Enum>().Where(e => value.HasFlag(e));

        public static string ToFormattedString(this Enum value)
        {
            string text = value.ToString();

            for (int i = 0; i < text.Length; i++)
            {
                char character = text[i];
                if (i == 0 || !char.IsLetter(character) || char.IsLower(character)) continue;

                text = text.Insert(i, " ");
                i++;
            }

            return text;
        }
    }
}