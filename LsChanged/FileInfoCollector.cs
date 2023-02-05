using LsChanged.Settings;

namespace LsChanged;

internal class FileInfoCollector
{
    private readonly FileInfoCollectorSettings _settings;

    public FileInfoCollector(FileInfoCollectorSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _settings = settings;
    }


    public IReadOnlyDictionary<string, FileInformation> Collect(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        var result = new Dictionary<string, FileInformation>();

        string[] filePaths;
        try
        {
            filePaths = Directory.GetFiles(path);
        }
        catch (UnauthorizedAccessException)
        {
            return result.AsReadOnly();
        }
        catch(DirectoryNotFoundException)
        {
            return result.AsReadOnly();
        }

        foreach (string filePath in filePaths)
        {
            FileInfo? fileInfo;
            FileInformation? fileInformation;
            try
            {
                fileInfo = new FileInfo(filePath);
                fileInformation = new FileInformation(fileInfo.Length,
                                                      fileInfo.LastWriteTimeUtc,
                                                      (int)fileInfo.Attributes,
                                                      (int)fileInfo.UnixFileMode);
                Console.WriteLine(filePath);
            }
            catch (FileNotFoundException)
            {
                continue;
            }
            result.TryAdd(filePath, fileInformation);
        }


        string[] directories = Directory.GetDirectories(path);
        foreach (string directoryPath in directories)
        {
            var dirInfo = new DirectoryInfo(directoryPath);
            if (!FollowDirectory(dirInfo))
            {
                continue;
            }

            var fileEntries = Collect(directoryPath);
            foreach (var entry in fileEntries)
            {
                result.TryAdd(entry.Key, entry.Value);
            }
        }

        return result.AsReadOnly();
    }

    private bool FollowDirectory(DirectoryInfo info)
    {
        if (_settings.FollowSymlinkSettings == FollowSymlinkSettings.Follow)
        {
            return true;
        }

        string? linkTarget = info.LinkTarget;
        if (linkTarget == null)
        {
            return true;
        }

        if (_settings.FollowSymlinkSettings == FollowSymlinkSettings.SkipAll)
        {
            return false;
        }

        bool isRecursive = info.FullName.StartsWith(linkTarget, StringComparison.OrdinalIgnoreCase);
        return !isRecursive;
    }
}
