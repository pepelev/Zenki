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
            Trigram.StringToTrigrams(s).Should().HaveCount(0);
        }

        [Test]
        public void Give_One_Trigram_From_Three_Char_String()
        {
            var s = "abc";
            Trigram.StringToTrigrams(s).Should().BeEquivalentTo(new [] { ('a', 'b', 'c').AsRunes() });
        }

        [Test]
        public void Give_Four_Trigrams_From_Six_Char_String()
        {
            var s = "abcdef";
            Trigram.StringToTrigrams(s).Should().HaveCount(4);
        }

        [Test]
        public void Give_Trigrams_With_WhiteSpace()
        {
            var s = "a bc";
            Trigram.StringToTrigrams(s).Should().HaveCount(1);
            Trigram.StringToTrigrams(s).Should().BeEquivalentTo(new [] { ('a', ' ', 'b').AsRunes() });
        }

        [Test]
        public void Remove_Multiple_Whitespaces()
        {
            var s = "a     bc";
            Trigram.StringToTrigrams(s).Should().HaveCount(1);
            Trigram.StringToTrigrams(s).Should().BeEquivalentTo(new [] { ('a', ' ', 'b').AsRunes() });
        }

        [Test]
        public void Single_Letter_With_Multiple_Whitespaces()
        {
            var s = "a      ";
            Trigram.StringToTrigrams(s).Should().HaveCount(0);
        }
    }
}