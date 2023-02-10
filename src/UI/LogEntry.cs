using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Zenki.UI;

public sealed class LogEntry
{
    public LogEntry(string raw, IReadOnlyCollection<(string Name, string Value)> properties)
    {
        Raw = raw;
        Properties = properties;
    }

    public string Raw { get; }
    public IReadOnlyCollection<(string Name, string Value)> Properties { get; }

    public override string ToString() => Raw;

    public string PropertiesString => string.Join("; ", Properties);

    public sealed class Structure
    {
        private static readonly Regex severity = new(
            @"\[(?<Severity>...)\]",
            RegexOptions.CultureInvariant | RegexOptions.Compiled
        );

        private static readonly Regex colonProperty = new(
            @"(?<Name>\w+)\: +\[?""?(?<Value>[^"",]+)""?\]?,?",
            RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.Multiline
        );

        public LogEntry Parse(string text)
        {
            var properties = new List<(string Name, string Value)>();
            if (severity.Match(text) is { Success: true } match)
            {
                properties.Add(("Severity", match.Groups["Severity"].Value));
            }

            foreach (Match propertyMatch in colonProperty.Matches(text))
            {
                var name = propertyMatch.Groups["Name"].Value;
                var value = propertyMatch.Groups["Value"].Value;
                properties.Add((name, value));
            }

            return new LogEntry(text, properties);
        }
    }

    public sealed class StartMark
    {
        private readonly Regex pattern;

        public StartMark(Regex pattern, int maxMatchLength)
        {
            MaxMatchLength = maxMatchLength;
            this.pattern = pattern;
        }

        public int MaxMatchLength { get; }

        public static StartMark Default { get; } = new(
            new Regex(
                // 01:35:31.678 [INF]
                @"^\d\d:\d\d:\d\d\.\d\d\d \[...\]",
                RegexOptions.Multiline | RegexOptions.Compiled
            ),
            maxMatchLength: 20
        );

        public IEnumerable<int> Find(string text) =>
            pattern.Matches(text).Select(match => match.Index);
    }
}