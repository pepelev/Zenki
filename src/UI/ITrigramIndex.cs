using System.Collections.Generic;

namespace Zenki.UI;

public interface ITrigramIndex<TKey, TValue>
{
    void Add((TKey, TKey, TKey) trigram, TValue value);

    IEnumerable<TValue> Search((TKey, TKey, TKey) trigram);
}
