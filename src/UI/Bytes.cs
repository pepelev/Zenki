using System;

namespace Zenki.UI;

public readonly struct Bytes
{
    private readonly unsafe byte* pointer;
    public long Length { get; }

    public unsafe Bytes(byte* pointer, long length)
    {
        if (pointer == (byte*)0)
        {
            throw new ArgumentException("null", nameof(pointer));
        }

        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, "negative");
        }

        this.pointer = pointer;
        Length = length;
    }

    public unsafe ReadOnlySpan<byte> HeadSpan => new(
        pointer,
        (int)Math.Min(Length, int.MaxValue)
    );

    public unsafe Bytes Slice(long offset)
    {
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "negative");
        }

        if (offset > Length)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), offset, $"greater than {nameof(Length)}");
        }

        return new Bytes(pointer + offset, Length - offset);
    }

    public unsafe Bytes Cut(long offset, long end)
    {
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "is negative");
        }

        if (end > Length)
        {
            throw new ArgumentOutOfRangeException(nameof(end), end, $"greater than {nameof(Length)}");
        }

        if (offset > end)
        {
            throw new ArgumentOutOfRangeException($"({nameof(offset)}, {nameof(end)})", (offset, end), "wrong");
        }

        return new Bytes(pointer + offset, end - offset);
    }

    public unsafe Bytes Truncate(long newLength)
    {
        var length = Math.Min(Length, newLength);
        return new Bytes(pointer, length);
    }
}