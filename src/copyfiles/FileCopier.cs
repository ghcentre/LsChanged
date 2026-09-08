using CopyFiles.Exceptions;

namespace CopyFiles;

internal static class FileCopier
{
    public static void CopyFiles(string sourceDirectory, string destinationDirectory, IEnumerable<string> relativeFilePaths)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);
        ArgumentNullException.ThrowIfNull(relativeFilePaths);

        DirectoryChecker.CheckDirectoryExists(sourceDirectory);
        DirectoryChecker.CheckDirectoryReadable(sourceDirectory);

        DirectoryChecker.CheckDirectoryExists(destinationDirectory);
        DirectoryChecker.CheckDirectoryWritable(destinationDirectory);

        Console.WriteLine($"Copying files from '{sourceDirectory}' to '{destinationDirectory}'...");

        int copiedCount = 0;

        foreach (string filePath in relativeFilePaths)
        {
            try
            {
                CopyFile(sourceDirectory, destinationDirectory, filePath);
                copiedCount++;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"Could not copy file '{filePath}': {exception.Message}");
            }
        }
    }

    private static void CopyFile(string sourceDirectory, string destinationDirectory, string filePath)
    {
        if (Path.IsPathFullyQualified(filePath) || Path.IsPathRooted(filePath))
        {
            throw new InvalidRelativeFilePathException(filePath);
        }

        string sourceFilePath = Path.Combine(sourceDirectory, filePath);
        if (!File.Exists(sourceFilePath))
        {
            throw new FileDoesNotExistException(sourceFilePath);
        }
        sourceFilePath = PrependWithExtendedLengthPrefix(sourceFilePath);

        string destinationFilePath = Path.Combine(destinationDirectory, filePath);
        destinationFilePath = PrependWithExtendedLengthPrefix(destinationFilePath);
        CreateDirectoryForFile(destinationFilePath);

        Console.Write($"Copying '{sourceFilePath}' to '{destinationFilePath}'...");
        File.Copy(sourceFilePath, destinationFilePath, true);
        Console.WriteLine("done.");
    }

    private static void CreateDirectoryForFile(string destinationFilePath)
    {
        string? directory = Path.GetDirectoryName(destinationFilePath);
        if (string.IsNullOrEmpty(directory))
        {
            return;
        }
            
        if (!Directory.Exists(directory))
        {
            Console.WriteLine($"Creating directory: '{directory}'.");
            Directory.CreateDirectory(directory);
        }
    }

    private static string PrependWithExtendedLengthPrefix(string path)
    {
        if (!OperatingSystem.IsWindows())
        {
            return path;
        }

        if (path.StartsWith(@"\\"))
        {
            return string.Concat(@"\\?\UNC\", path.AsSpan(2));
        }

        return @"\\?\" + path;
    }
}
