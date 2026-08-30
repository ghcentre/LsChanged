using LsChanged.Logging;
using System.Diagnostics;

namespace LsChanged.Collector;

internal sealed class FileInfoCollector(ILogger logger, FollowSymlinksMode followSymlinksMode) : IFileInfoCollector
{
    private const int _initialCapacity = 1024;
    private readonly Dictionary<string, FileStatus> _files = new(_initialCapacity);
    private readonly HashSet<string> _visitedDirectories = new(_initialCapacity);

    private int _totalFiles;
    private int _totalDirectories;

    public IReadOnlyDictionary<string, FileStatus> Collect(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        _files.Clear();
        _visitedDirectories.Clear();
        _totalFiles = _totalDirectories = 0;

        CollectRecursive(path);

        logger.Debug(
            Environment.NewLine + "Visited {0} folder(s). Collected {1} file(s).",
            _totalDirectories,
            _totalFiles);

        return _files.AsReadOnly();
    }


    private void CollectRecursive(string path)
    {
        logger.Debug(path);

        CollectFiles(path);
        _totalDirectories++;

        var directories = GetDirectories(path);

        foreach (string directoryPath in directories)
        {
            if (followSymlinksMode == FollowSymlinksMode.Follow)
            {
                CollectRecursive(directoryPath);
                continue;
            }

            string? linkTarget = ResolveLinkTarget(directoryPath);

            if (followSymlinksMode == FollowSymlinksMode.Skip)
            {
                if (linkTarget == null)
                {
                    CollectRecursive(directoryPath);
                }

                continue;
            }

            Debug.Assert(followSymlinksMode == FollowSymlinksMode.PreventRecursion);

            string checkee = linkTarget ?? directoryPath;
            if (_visitedDirectories.Contains(checkee))
            {
                continue;
            }

            _visitedDirectories.Add(checkee);

            CollectRecursive(directoryPath);
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

            bool added = _files.TryAdd(filePath, status);
            if (added)
            {
                _totalFiles++;
            }
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
            return [];
        }
        catch (DirectoryNotFoundException)
        {
            return [];
        }
        catch (IOException)
        {
            return [];
        }
    }

    private static FileStatus? CreateFileStatus(string filePath)
    {
        try
        {
            var fileInfo = new FileInfo(filePath);

            return new FileStatus(
                fileInfo.Length,
                fileInfo.LastWriteTimeUtc,
                (int)fileInfo.Attributes,
                (int)fileInfo.UnixFileMode);
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
            return [];
        }
        catch (DirectoryNotFoundException)
        {
            return [];
        }
    }

    private static string? ResolveLinkTarget(string directoryPath)
    {
        try
        {
            var link = Directory.ResolveLinkTarget(directoryPath, true) as DirectoryInfo;
            return link?.FullName;
        }
        catch (DirectoryNotFoundException)
        {
            return null;
        }
    }

    #endregion
}
