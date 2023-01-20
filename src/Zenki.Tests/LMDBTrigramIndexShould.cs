using FluentAssertions;
using NUnit.Framework;
using Zenki.UI;

namespace Zenki.Tests
{
    public sealed class LMDBTrigramIndexShould
    {
        const string testDbName = "test";

        [SetUp]
        public void SetUp()
        {
            if (Directory.Exists(testDbName))
                Directory.Delete(testDbName, true);
        }

        [Test]
        public void Give_Nothing_When_Nothing_Added()
        {
            using var sut = new LMDBTrigramIndex(testDbName);

            var trigram = ('a', 'b', 'c');

            sut.Search(trigram).Should().BeEmpty();

        }

        [Test]
        public void Give_Added_Value()
        {
            using var sut = new LMDBTrigramIndex(testDbName);

            var trigram = ('a', 'b', 'c');
            var value = 10;

            sut.Add(trigram, value);
            sut.Search(trigram).Should().BeEquivalentTo(new[] { value });
        }

        [Test]
        public void Give_Multiple_Added_Values()
        {
            using var sut = new LMDBTrigramIndex(testDbName);

            var trigram = ('a', 'b', 'c');
            var value1 = 10L;
            var value2 = 20L;

            sut.Add(trigram, value1);
            sut.Add(trigram, value2);
            sut.Search(trigram).Should().BeEquivalentTo(new[] { value1, value2 });
        }
    }
}