using System.Text;

namespace Zenki.UI;

public static class TrigramExtensions
{
    public static (Rune, Rune, Rune) AsRunes(this (char, char, char) trigram)
        => ((Rune)trigram.Item1, (Rune)trigram.Item2, (Rune)trigram.Item3);
}