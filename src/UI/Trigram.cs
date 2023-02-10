using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Zenki.UI;

public class Trigram
{
    private static readonly Regex MultipleWhitespaces = new Regex(@"\s+", RegexOptions.Compiled);
    public static IEnumerable<(Rune, Rune, Rune)> StringToTrigrams(string s)
    {
        s = MultipleWhitespaces.Replace(s, " ").ToLowerInvariant();

        if (s.Length < 3)
            yield break;

        var runes = s.EnumerateRunes().ToList();
        var count = runes.Count;
        
        for (var i = 2; i < count; i++)
        {
            if (!Rune.IsWhiteSpace(runes[i - 2]) && !Rune.IsWhiteSpace(runes[i]))
                yield return (runes[i - 2], runes[i - 1], runes[i]);
        }
    }

}