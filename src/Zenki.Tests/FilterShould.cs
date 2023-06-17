using FluentAssertions;
using NUnit.Framework;
using Sprache;
using Zenki.UI;

namespace Zenki.Tests;

public sealed class FilterShould
{
    private static readonly (string, string) errValue = ("ERR", "Wrong");
    private static readonly (string, string) infValue = ("INF", "Info");
    private readonly LogEntry emptyEntry = new("", new List<(string Name, string Value)>());
    private readonly LogEntry fullEntry = new("", new List<(string Name, string Value)> { errValue, infValue });
    private readonly LogEntry errEntry = new("", new List<(string Name, string Value)> { errValue });
    private readonly LogEntry infEntry = new("", new List<(string Name, string Value)> { infValue });
    private readonly FieldFilter errFilter = new("ERR", "Wrong");
    private readonly FieldFilter infFilter = new("INF", "Info");

    [Test]
    public void FindMatches()
    {
        errFilter.Pass(emptyEntry).Should().BeFalse();
        errFilter.Pass(errEntry).Should().BeTrue();
        errFilter.Pass(fullEntry).Should().BeTrue();
        infFilter.Pass(errEntry).Should().BeFalse();
    }

    [Test]
    public void Disjunct()
    {
        var orFilter = new OrFilter(errFilter, infFilter);
        orFilter.Pass(fullEntry).Should().BeTrue();
        orFilter.Pass(emptyEntry).Should().BeFalse();
        orFilter.Pass(errEntry).Should().BeTrue();
        orFilter.Pass(infEntry).Should().BeTrue();
    }

    [Test]
    public void Conjunct()
    {
        var andFilter = new AndFilter(errFilter, infFilter);
        andFilter.Pass(fullEntry).Should().BeTrue();
        andFilter.Pass(emptyEntry).Should().BeFalse();
        andFilter.Pass(infEntry).Should().BeFalse();
        andFilter.Pass(errEntry).Should().BeFalse();
    }
}