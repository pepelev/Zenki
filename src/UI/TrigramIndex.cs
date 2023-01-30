using System;
using System.Collections.Generic;
using System.Linq;

namespace Zenki.UI;

public sealed class TrigramIndex<TKey, TValue> : ITrigramIndex<TKey, TValue>
{
    // public int linesCount = 1;
    private Dictionary<(TKey, TKey, TKey), Dictionary<TValue, double>> indexes = new(); //key is value; value is weight

    public void Add((TKey, TKey, TKey) trigram, TValue value)
    {
        if (indexes.ContainsKey(trigram))
        {
            if (indexes[trigram].ContainsKey(value))
                indexes[trigram][value]++;
            else
                indexes[trigram][value] = 0;
        }
        else
        {
            indexes[trigram] = new Dictionary<TValue, double>
            {
                { value, 0 }
            };
        }
    }

    public IEnumerable<TValue> Search((TKey, TKey, TKey) trigram)
    {
        if (indexes.TryGetValue(trigram, out var value))
        {
            //var idf = Math.Log((double)linesCount / indexes[trigram].Count);
            return value
                .OrderByDescending(x => x.Value)
                .Select(x=>x.Key)
                .ToList();
        }
        return Enumerable.Empty<TValue>();
    }

    public double GetWeight((TKey, TKey, TKey) trigram)
    {
        return indexes[trigram].Count;
    }
}