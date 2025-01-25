using System.Collections.Generic;
using System.Globalization;

namespace System;

internal static class StringExtensions
{
    public static IEnumerable<string> EnumerateTextElements(this string text)
    {
        for (var en = StringInfo.GetTextElementEnumerator(text); en.MoveNext();)
        {
            yield return en.GetTextElement();
        }
    }
}
