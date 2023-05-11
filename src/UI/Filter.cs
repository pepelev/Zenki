using System;

namespace Zenki.UI;

public abstract class Filter
{
    public abstract bool Pass(LogEntry entry);
}