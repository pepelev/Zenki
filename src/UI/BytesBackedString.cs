using System;
using System.Buffers;
using System.Text;

namespace Zenki.UI;

public readonly struct BytesBackedString
{
    private readonly FrozenList<int> offsets;

    private BytesBackedString(FrozenList<int> offsets, string value)
    {
        this.offsets = offsets;
        Value = value;
    }

    public string Value { get; }
    public int GetOffset(int charIndex) => offsets.Span[charIndex];
    public override string ToString() => Value;

    public void ReturnBuffer(ArrayPool<int> pool)
    {
        offsets.ReturnBuffer(pool);
    }

    public static BytesBackedString Utf8(Bytes bytes, ArrayPool<int> pool)
    {
        var span = bytes.HeadSpan;
        if (bytes.Length > span.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes), "too large");
        }

        const int utf8MaxBytesPerChar = 4;
        var startIndex = GetStartIndex(span[..Math.Min(utf8MaxBytesPerChar * 2, span.Length)]);
        var offset = startIndex;
        var stringLength = 0;
        var offsets = new FrozenList<int>.Builder(pool);
        while (true)
        {
            var status = Rune.DecodeFromUtf8(span[offset..], out var rune, out var bytesConsumed);
            if (status == OperationStatus.Done)
            {
                stringLength += rune.Utf16SequenceLength;
                for (var i = 0; i < rune.Utf16SequenceLength; i++)
                {
                    offsets.Add(offset);
                }

                offset += bytesConsumed;
            }
            else if (status == OperationStatus.NeedMoreData)
            {
                break;
            }
            else
            {
                throw new Exception();
            }
        }

        var value = string.Create(
            stringLength,
            bytes.Slice(startIndex),
            static (chars, localMemory) =>
            {
                var localSpan = localMemory.HeadSpan;
                while (true)
                {
                    var status = Rune.DecodeFromUtf8(localSpan, out var rune, out var bytesConsumed);
                    if (status == OperationStatus.Done)
                    {
                        var charsWritten = rune.EncodeToUtf16(chars);
                        chars = chars[charsWritten..];
                        localSpan = localSpan[bytesConsumed..];
                        continue;
                    }

                    return;
                }
            }
        );
        return new BytesBackedString(offsets.Build(), value);
    }

    private static int GetStartIndex(ReadOnlySpan<byte> content)
    {
        for (var i = 0; i < content.Length; i++)
        {
            if (Rune.DecodeFromUtf8(content[i..], out _, out _) == OperationStatus.Done)
            {
                return i;
            }
        }

        throw new Exception("Could not find start");
    }
}