using System;

namespace Zenki.UI;

public abstract class Filter
{
    public abstract bool Pass(LogEntry entry);
}

public sealed class FieldFilter : Filter
{
    private readonly string field;
    private readonly string value;
    
    public override bool Pass(LogEntry entry)
    {
        // if (entry.Properties...)
        throw new NotImplementedException();
    }
}

public sealed class AndFilter : Filter
{
    private readonly Filter a;
    private readonly Filter b;

    public override bool Pass(LogEntry entry)
    {
        throw new NotImplementedException();
    }
}

// public sealed class OrFilter : Filter