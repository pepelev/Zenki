using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zenki.UI;

public sealed class LogFile : IEnumerable<(long Offset, LogEntry Entry)>
{
    private readonly File file;

    public LogFile(File file)
    {
        this.file = file;
    }

    public IEnumerator<(long Offset, LogEntry Entry)> GetEnumerator()
    {
        var chunk = file.Slice(0);
        var bytes = chunk.Bytes;
        var mark = LogEntry.StartMark.Default;
        var partSize = Math.Max(mark.MaxMatchLength, 16 * 1024);
        var logStarts = new HashSet<long>();
        var pool = ArrayPool<int>.Shared;
        for (var offset = 0L; offset < bytes.Length; offset += partSize)
        {
            var left = Math.Max(0, offset - mark.MaxMatchLength);
            var right = Math.Min(bytes.Length, offset + partSize);
            var part = bytes.Cut(left, right);
            var @string = BytesBackedString.Utf8(part, pool);
            var indexes = mark.Find(@string.Value);
            foreach (var index in indexes)
            {
                logStarts.Add(left + @string.GetOffset(index));
            }

            @string.ReturnBuffer(pool);
        }

        var structure = new LogEntry.Structure();
        var sortedLogStarts = logStarts.OrderBy(x => x).ToList();
        for (var i = 0; i < sortedLogStarts.Count - 1; i++)
        {
            var str = BytesBackedString.Utf8(bytes.Cut(sortedLogStarts[i], sortedLogStarts[i + 1]), pool);
            yield return (sortedLogStarts[i], structure.Parse(str.Value));

            str.ReturnBuffer(pool);
        }

        var lastStr = BytesBackedString.Utf8(bytes.Slice(sortedLogStarts[^1]), pool);
        yield return (sortedLogStarts[^1], structure.Parse(lastStr.Value));

        lastStr.ReturnBuffer(pool);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}