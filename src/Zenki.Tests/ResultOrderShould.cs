using FluentAssertions;
using NUnit.Framework;
using Zenki.UI;

namespace Zenki.Tests;

public sealed class ResultOrderShould
{
    [Test]
    public void CompareCorrectly()
    {
        var x = "example";
        var y = "exampeexampampl";
        var query = "exampl";

        var comp = new ResultOrder(query);

        comp.Compare(x, y).Should().Be(-1);
    }
}