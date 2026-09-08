using CopyFiles.Exceptions;

namespace CopyFiles;

internal class ListFileReader
{
    public static IEnumerable<string> GetFilePaths(string listFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(listFilePath);

        if (!File.Exists(listFilePath))
        {
            throw new FileDoesNotExistException(listFilePath);
        }

        foreach (string line in GetLines(listFilePath))
        {
            yield return line;
        }
    }

    private static IEnumerable<string> GetLines(string path)
    {
        try
        {
            return File.ReadLines(path);
        }
        catch (UnauthorizedAccessException uae)
        {
            throw new FileNotReadableException(path, uae);
        }
        catch (IOException ioe)
        {
            throw new IOErrorException(path, ioe);
        }
    }
}
