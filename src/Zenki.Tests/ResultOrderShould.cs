using FluentAssertions;
using NUnit.Framework;
using Zenki.UI;

namespace Zenki.Tests;

public sealed class ResultOrderShould
{
    [Test]
    public void CompareCorrectly()
    {
        var list = new List<string> { "aaa", "abc", "baab" };
        var query = "aab";

        var resultOrder = new ResultOrder(query);
        
        list.Sort(resultOrder);

        list.Should().Equal(new List<string>{"baab", "abc", "aaa"});
    }
}