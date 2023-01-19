using System.Collections.Generic;
using System.Linq;

namespace Zenki.UI;

public sealed class TrigramIndex<TKey, TValue> : ITrigramIndex<TKey, TValue>
{
    public Dictionary<(TKey, TKey, TKey), List<TValue>> indexes = new();

    public void Add((TKey, TKey, TKey) trigram, TValue value)
    {
        if (indexes.ContainsKey(trigram))
            indexes[trigram].Add(value);
        else
            indexes.Add(trigram, new List<TValue> { value });
    }

    public IEnumerable<TValue> Search((TKey, TKey, TKey) trigram)
    {
        return indexes.TryGetValue(trigram, out var value)
            ? value.ToList()
            : Enumerable.Empty<TValue>();
    }
}