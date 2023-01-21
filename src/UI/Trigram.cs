using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Zenki.UI;

public class Trigram
{
    public static IEnumerable<(char, char, char)> StringToTrigrams(string s)
    {
        s = Regex.Replace(s, @"\s+", " ").ToLowerInvariant();
        
        if (s.Length < 3)
            yield break;

        for (var i = 2; i < s.Length; i++)
        {
            if (!char.IsWhiteSpace(s[i-2]))
                yield return (s[i-2], s[i-1], s[i]);
        }
    }
}
