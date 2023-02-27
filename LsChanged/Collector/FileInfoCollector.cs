using LsChanged.Logging;
using System.Diagnostics;

namespace LsChanged.Collector;

internal sealed class FileInfoCollector
{
    private readonly Dictionary<string, FileStatus> _files = new();
    private readonly HashSet<string> _visitedDirectories = new();

    private int _totalFiles;
    private int _totalDirectories;

    private readonly FollowSymlinksMode _followSymlinksMode;
    private readonly ILogger _logger;

    public FileInfoCollector(ILogger logger, FollowSymlinksMode followSymlinksMode)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;

        _followSymlinksMode = followSymlinksMode;
    }

    public IReadOnlyDictionary<string, FileStatus> Collect(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        _files.Clear();
        _visitedDirectories.Clear();
        _totalFiles = _totalDirectories = 0;

        LogAndCollectRecursive(path);

        _logger.Debug(
            System.Environment.NewLine + "Visited {0} folder(s). Collected {1} file(s).",
            _totalDirectories,
            _totalFiles);

        return _files.AsReadOnly();
    }


    private void CollectRecursive(string path)
    {
        _logger.Debug(path);

        CollectFiles(path);
        _totalDirectories++;

        var directories = GetDirectories(path);

        foreach (string directoryPath in directories)
        {
            if (_followSymlinksMode == FollowSymlinksMode.Follow)
            {
                CollectRecursive(directoryPath);
                continue;
            }

            string? linkTarget = ResolveLinkTarget(directoryPath);

            if (_followSymlinksMode == FollowSymlinksMode.Skip)
            {
                if (linkTarget == null)
                {
                    CollectRecursive(directoryPath);
                }

                continue;
            }

            Debug.Assert(_followSymlinksMode == FollowSymlinksMode.PreventRecursion);

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
        catch (DirectoryNotFoundException)
        {
            return Enumerable.Empty<string>();
        }
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
