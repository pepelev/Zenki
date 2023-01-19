using NUnit.Framework;
using Zenki.UI;

namespace Zenki.Tests
{
    public class Class1
    {
        [Test]
        public void Test()
        {
            var sut = new TrigramIndex<char, int>();

            sut.Add(('a', 'b', 'c'), 42);
            var output = sut.Search(('a', 'b', 'c'));
            foreach (var item in output)
            {
                sut.Add(('a', 'b', 'c'), 43);

            }
        }
    }
}