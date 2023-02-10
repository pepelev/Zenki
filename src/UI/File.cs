using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Zenki.UI;

public sealed class File : IDisposable
{
    private readonly FileStream stream;

    public File(FileStream stream)
    {
        this.stream = stream;
    }

    public long Size => stream.Length;

    public Chunk Slice(long offset)
    {
        if (stream.Length == 0)
        {
            throw new NotImplementedException();
        }

        var map = MemoryMappedFile.CreateFromFile(
            stream,
            null,
            0L, // 0 means stream.Length
            MemoryMappedFileAccess.Read,
            HandleInheritability.None,
            true
        );
        var view = map.CreateViewAccessor(offset, 0, MemoryMappedFileAccess.Read);
        return new Chunk(stream, map, view);
    }

    public sealed class Chunk : IDisposable
    {
        private readonly FileStream file;
        private readonly MemoryMappedFile map;
        private readonly MemoryMappedViewAccessor view;

        public Chunk(FileStream file, MemoryMappedFile map, MemoryMappedViewAccessor view)
        {
            this.file = file;
            this.map = map;
            this.view = view;
        }

        public long Offset => view.PointerOffset;

        // todo use file.Length to not show trailing nulls
        public long Size => view.Capacity;

        public unsafe Bytes Bytes => new(
            (byte*)view.SafeMemoryMappedViewHandle.DangerousGetHandle().ToPointer(),
            Size
        );

        public void Dispose()
        {
            view.Dispose();
            map.Dispose();
        }
    }

    public void Dispose()
    {
        stream.Dispose();
    }
}