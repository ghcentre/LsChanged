using System.IO.Compression;

namespace GzipLab;

internal class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: GzipLab file[.gz]");
            return 1;
        }   
        
        if (args[0].EndsWith(".gz"))
        {
            Decompress(args[0]);
            return 0;
        }

        Compress(args[0]);
        return 0;
    }

    private static void Compress(string path)
    {
        using var originalFileStream = File.OpenRead(path);
        using var compressedFileStream = File.Create(path + ".gz");
        using var compressor = new GZipStream(compressedFileStream, CompressionLevel.SmallestSize);
        originalFileStream.CopyTo(compressor);
    }

    private static void Decompress(string path)
    {
        using var compressedFileStream = File.OpenRead(path);
        using var decompressedFileStream = File.Create(path.Substring(0, path.LastIndexOf(".gz")));
        using var decompressor = new GZipStream(compressedFileStream, CompressionMode.Decompress);
        decompressor.CopyTo(decompressedFileStream);
    }
}