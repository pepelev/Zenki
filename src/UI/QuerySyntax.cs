using Sprache;

namespace Zenki.UI;

public static class QuerySyntax
{
    public static readonly Parser<FieldFilter> FieldParser =
        from field in Parse.Letter.AtLeastOnce().Text().Token()
        from eq in Parse.Char('=')
        from value in Parse.Letter.AtLeastOnce().Text().Token()
        select new FieldFilter(field, value);
}