using System;
using System.Linq;

namespace Zenki.UI;

public abstract class Filter
{
    public abstract bool Pass(LogEntry entry);
}

public sealed class FieldFilter : Filter
{
    private readonly string field;
    private readonly string value;

    public FieldFilter(string field, string value)
    {
        this.field = field;
        this.value = value;
    }
    
    public override bool Pass(LogEntry entry)
    {
        return entry.Properties.Contains((field, value));
    }
}

public sealed class AndFilter : Filter
{
    private readonly Filter a;
    private readonly Filter b;

    public AndFilter(Filter b, Filter a)
    {
        this.b = b;
        this.a = a;
    }

    public override bool Pass(LogEntry entry) => a.Pass(entry) & b.Pass(entry);

}

public sealed class OrFilter : Filter
{
    private readonly Filter a;
    private readonly Filter b;

    public OrFilter(Filter a, Filter b)
    {
        this.a = a;
        this.b = b;
    }

    public override bool Pass(LogEntry entry) => a.Pass(entry) | b.Pass(entry);
}