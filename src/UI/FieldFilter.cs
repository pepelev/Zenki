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
}