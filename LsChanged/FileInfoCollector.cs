using LsChanged.Settings;
using System.Diagnostics;

namespace LsChanged;

internal class FileInfoCollector
{
    private readonly Dictionary<string, FileStatus> _files = new();
    private readonly HashSet<string> _visitedDirectories = new();

    private readonly FileInfoCollectorSettings _settings;

    public FileInfoCollector(FileInfoCollectorSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _settings = settings;
    }

    public IReadOnlyDictionary<string, FileStatus> Collect(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        _files.Clear();
        _visitedDirectories.Clear();

        CollectRecursive(path);

        return _files.AsReadOnly();
    }


    private void CollectRecursive(string path)
    {
        CollectFiles(path);

        var directories = GetDirectories(path);

        foreach (string directoryPath in directories)
        {
            if (_settings.FollowSymlinkSettings == FollowSymlinkSettings.Follow)
            {
                LogAndCollectRecursive(directoryPath);
                continue;
            }

            string? linkTarget = ResolveLinkTarget(directoryPath);

            if (_settings.FollowSymlinkSettings == FollowSymlinkSettings.SkipAll)
            {
                if (linkTarget == null)
                {
                    LogAndCollectRecursive(directoryPath);
                }

                continue;
            }

            Debug.Assert(_settings.FollowSymlinkSettings == FollowSymlinkSettings.SkipRecirsive);

            string checkee = linkTarget ?? directoryPath;
            if (_visitedDirectories.Contains(checkee))
            {
                continue;
            }

            _visitedDirectories.Add(checkee);

            LogAndCollectRecursive(directoryPath);
        }
    }


    #region Files

    private void CollectFiles(string path)
    {
        var filePaths = GetFiles(path);

        foreach (string filePath in filePaths)
        {
            var status = CreateFileStatus(filePath);
            if (status == null)
            {
                continue;
            }

            _files.TryAdd(filePath, status);
        }
    }

    private static IEnumerable<string> GetFiles(string path)
    {
        try
        {
            string[] result = Directory.GetFiles(path);
            return result;
        }
        catch (UnauthorizedAccessException)
        {
            return Enumerable.Empty<string>();
        }
        catch (DirectoryNotFoundException)
        {
            return Enumerable.Empty<string>();
        }
    }

    private static FileStatus? CreateFileStatus(string filePath)
    {
        try
        {
            var fileInfo = new FileInfo(filePath);

            var status = new FileStatus(
                fileInfo.Length,
                fileInfo.LastWriteTimeUtc,
                (int)fileInfo.Attributes,
                (int)fileInfo.UnixFileMode);

            return status;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    #endregion

    #region Directories

    private static IEnumerable<string> GetDirectories(string path)
    {
        try
        {
            string[] result = Directory.GetDirectories(path);
            return result;
        }
        catch (UnauthorizedAccessException)
        {
            return Enumerable.Empty<string>();
        }
        catch(DirectoryNotFoundException)
        {
            return Enumerable.Empty<string>();
        }
    }

    private void LogAndCollectRecursive(string directoryPath)
    {
        Console.WriteLine(directoryPath);
        CollectRecursive(directoryPath);
    }

    private static string? ResolveLinkTarget(string directoryPath)
    {
        try
        {
            var link = Directory.ResolveLinkTarget(directoryPath, true) as DirectoryInfo;

            string? result = link?.FullName;
            return result;
        }
        catch (DirectoryNotFoundException)
        {
            return null;
        }
    }

    #endregion
}
