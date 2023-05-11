namespace Zenki.UI;

public sealed class AndFilter : Filter
{
    private readonly Filter a;
    private readonly Filter b;

    public AndFilter(Filter b, Filter a)
    {
        this.b = b;
        this.a = a;
    }

    public override bool Pass(LogEntry entry) => a.Pass(entry) && b.Pass(entry);
}