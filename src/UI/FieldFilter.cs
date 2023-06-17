using System;
using System.Linq;

namespace Zenki.UI;

public sealed class FieldFilter : Filter
{
    private readonly string field;
    private readonly string value;

    public FieldFilter(string field, string value)
    {
        this.field = field;
        this.value = value;
    }

    public override bool Pass(LogEntry entry) => entry.Properties.Contains((field, value));

    private bool Equals(FieldFilter other) => field == other.field && value == other.value;

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || (obj is FieldFilter other && Equals(other));

    public override int GetHashCode() => HashCode.Combine(field, value);
}