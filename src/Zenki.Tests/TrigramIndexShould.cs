using NUnit.Framework;
using Zenki.UI;

namespace Zenki.Tests;

public sealed class TrigramIndexShould
{
    [Test]
    public void NotOverrideValues()
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