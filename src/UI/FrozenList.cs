using System;
using System.Buffers;
using JetBrains.Annotations;

namespace Zenki.UI;

public readonly struct FrozenList<T>
{
    private readonly T[] content;
    private readonly int count;

    private FrozenList(T[] content, int count)
    {
        this.content = content;
        this.count = count;
    }

    public ReadOnlySpan<T> Span => content switch
    {
        null => ReadOnlySpan<T>.Empty,
        { } array => new ReadOnlySpan<T>(array, 0, count)
    };

    public void ReturnBuffer(ArrayPool<T> pool)
    {
        pool.Return(content);
    }

    public struct Builder
    {
        private readonly ArrayPool<T> pool;
        private T[] content = Array.Empty<T>();
        private int count = 0;

        public Builder(ArrayPool<T> pool)
        {
            this.pool = pool;
        }

        public void Add(T item)
        {
            if (count == content.Length)
            {
                var newContent = pool.Rent(Math.Max(4, count * 2));
                content.CopyTo(newContent.AsSpan());
                pool.Return(content);
                content = newContent;
            }

            content[count++] = item;
        }

        [MustUseReturnValue]
        public FrozenList<T> Build()
        {
            var result = new FrozenList<T>(content, count);
            content = Array.Empty<T>();
            count = 0;
            return result;
        }
    }
}