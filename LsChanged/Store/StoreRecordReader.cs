using System.IO.Compression;
using System.Text;

namespace LsChanged.Store;

internal class StoreRecordReader : IStoreRecordReader
{
    private const string _gzipExtension = ".gz";

    private readonly IStoreRecordDeserializer _deserializer;

    public StoreRecordReader(IStoreRecordDeserializer deserializer)
    {
        _deserializer = deserializer;
    }

    public IStoreRecord Read(IStoreEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        bool useCompression = entry.Id.EndsWith(_gzipExtension);
        string filePath = entry.Id;

        string serialized = useCompression ? ReadCompressed(filePath) : ReadUncompressed(filePath);

        var result = _deserializer.Deserialize(serialized);
        return result;
    }


    private static string ReadUncompressed(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    private static string ReadCompressed(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);
        using var decompressedStream = new MemoryStream();
        using var decompressor = new GZipStream(fileStream, CompressionMode.Decompress);
        decompressor.CopyTo(decompressedStream);

        decompressedStream.Seek(0, SeekOrigin.Begin);
        var bytes = decompressedStream.ToArray();

        string result = Encoding.UTF8.GetString(bytes);
        return result;
    }
}
