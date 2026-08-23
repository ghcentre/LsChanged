using System.IO.Compression;
using System.Text;
using LsChanged.Store.Abstractions;

namespace LsChanged.Store;

internal class StoreRecordWriter : IStoreRecordWriter
{
    private const string _gzipExtension = ".gz";

    private readonly IStoreRecordSerializer _serializer;

    public StoreRecordWriter(IStoreRecordSerializer serializer)
    {
        _serializer = serializer;
    }

    public IStoreEntry Write(IStoreEntry entry, IStoreRecord storeRecord)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(storeRecord);

        string fileName = entry.Id;
        if (!fileName.EndsWith(_gzipExtension))
        {
            fileName += _gzipExtension;
        }

        string filePath = fileName;
        byte[] serializedBytes = _serializer.Serialize(storeRecord);

        using var originalStream = new MemoryStream(serializedBytes);
        using var fileStream = File.Create(filePath);

        using var compressor = new GZipStream(fileStream, CompressionLevel.SmallestSize);
        originalStream.CopyTo(compressor);

        var result = new StoreEntry(fileName);
        return result;
    }
}
