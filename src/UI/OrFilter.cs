namespace Zenki.UI;

public sealed class OrFilter : Filter
{
    private readonly Filter a;
    private readonly Filter b;

    public OrFilter(Filter a, Filter b)
    {
        this.a = a;
        this.b = b;
    }

    public override bool Pass(LogEntry entry) => a.Pass(entry) || b.Pass(entry);
}