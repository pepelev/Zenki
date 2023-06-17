using FluentAssertions;
using NUnit.Framework;
using Sprache;
using Zenki.UI;

namespace Zenki.Tests;

public sealed class QuerySyntaxShould
{
    [Test]
    public void ParseFieldFilter()
    {
        const string example = " sadness  =mood  ";
        var filter = new FieldFilter("sadness", "mood");
        var spr = QuerySyntax.FieldParser.Parse(example);
        spr.Should().BeEquivalentTo(filter);
    }
}