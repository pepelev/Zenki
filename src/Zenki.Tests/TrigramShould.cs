using FluentAssertions;
using NUnit.Framework;
using Zenki.UI;

namespace Zenki.Tests
{
    public sealed class TrigramShould
    {
        [Test]
        public void Give_Nothing_From_Empty_String()
        {
            var s = "";
            Trigram.StringToTrigrams(s).Count().Should().Be(0);
        }

        [Test]
        public void Give_One_Trigram_From_Three_Char_String()
        {
            var s = "abc";
            // Trigram.StringToTrigrams(s).ToList().Should().BeEquivalentTo(new List<(char,char,char)> { ('a', 'b', 'c') });
        }

        [Test]
        public void Give_Four_Trigrams_From_Six_Char_String()
        {
            var s = "abcdef";
            Trigram.StringToTrigrams(s).Count().Should().Be(4);
        }

        [Test]
        public void Give_Trigrams_With_WhiteSpace()
        {
            var s = "a bc";
            Trigram.StringToTrigrams(s).Count().Should().Be(1);
            // Trigram.StringToTrigrams(s).ToList().Should().BeEquivalentTo(new List<(char, char, char)> { ('a', ' ', 'b') });
        }

        [Test]
        public void Remove_Multiple_Whitespaces()
        {
            var s = "a     bc";
            Trigram.StringToTrigrams(s).Count().Should().Be(1);
            // Trigram.StringToTrigrams(s).ToList().Should().BeEquivalentTo(new List<(char, char, char)> { ('a', ' ', 'b') });
        }

        [Test]
        public void Single_Letter_With_Multiple_Whitespaces()
        {
            var s = "a      ";
            Trigram.StringToTrigrams(s).Count().Should().Be(0);
        }
    }
}