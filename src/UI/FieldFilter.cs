using System.Linq;
using Sprache;

namespace Zenki.UI;

public sealed class FieldFilter : Filter
{
    private readonly string field;
    private readonly string value;

    private static readonly Parser<FieldFilter> FieldParser =
        from field in Parse.Letter.AtLeastOnce().Text().Token()
        from eq in Parse.Char('=')
        from value in Parse.Letter.AtLeastOnce().Text().Token()
        select new FieldFilter(field, value);

    public static FieldFilter SpracheParseField(string str) => FieldParser.Parse(str);

    public override bool Equals(object? obj)
    {
        var b = (FieldFilter)obj!;
        return b.field == field && b.value == value;
    }

    public FieldFilter(string field, string value)
    {
        this.field = field;
        this.value = value;
    }
    
    public override bool Pass(LogEntry entry) => entry.Properties.Contains((field, value));
}