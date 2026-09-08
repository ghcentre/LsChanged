using CopyFiles.Exceptions;

namespace CopyFiles;

internal static class DirectoryChecker
{
    public static void CheckDirectoryExists(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        
        if (!Directory.Exists(path))
        {
            throw new DirectoryDoesNotExistException(path);
        }
    }

    public static void CheckDirectoryReadable(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        
        try
        {
            var entries = Directory.EnumerateFileSystemEntries(path);
            var enumerator = entries.GetEnumerator();
            enumerator.MoveNext();
            return;
        }
        catch (UnauthorizedAccessException uae)
        {
            throw new DirectoryNotReadableException(path, uae);
        }
        catch (IOException ioe)
        {
            throw new IOErrorException(path, ioe);
        }
    }

    public static void CheckDirectoryWritable(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        
        try
        {
            string testFilePath = Path.Combine(path, Path.GetRandomFileName());
            using (FileStream fs = File.Create(testFilePath))
            {
                // Successfully created a file, so the directory is writable.
            }
            File.Delete(testFilePath);
        }
        catch (UnauthorizedAccessException uae)
        {
            throw new DirectoryNotWritableException(path, uae);
        }
        catch (IOException ioe)
        {
            throw new IOErrorException(path, ioe);
        }
    }
}
