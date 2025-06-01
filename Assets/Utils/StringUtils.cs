using System;

namespace Fulbo.Match
{
    public class StringUtils
    {
        public static string EnumToString<EnumType>(EnumType value) where EnumType : Enum
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