using LsChanged.Collector;
using LsChanged.CommandLine;
using LsChanged.Compare;
using LsChanged.Exceptions;
using LsChanged.Logging;
using LsChanged.Store.Abstractions;
using System.Text;

namespace LsChanged.ProgramRunner;

internal sealed class CompareStrategy : IRunnerStrategy
{
    private readonly ILogger _logger;
    private readonly CommandLineOptions _options;
    private readonly IStore _store;

    private bool _includeAdded;
    private bool _includeModified;
    private bool _includeUnmodified;
    private bool _includeDeleted;

    private int _numAdded;
    private int _numModified;
    private int _numUnmodified;
    private int _numDeleted;

    public CompareStrategy(ILogger logger, CommandLineOptions options, IStore store)
    {
        _logger = logger;
        _options = options;
        _store = store;

        ConfigureIncludeSwitches(options);
    }

    public int Run()
    {
        _numAdded = _numModified = _numUnmodified = _numDeleted = 0;

        var entries = _store.ListAll().ToList();

        (int newOrdinal, int? oldOrdinal) = GetSnapshotOrdinals(entries);

        var compared = CompareSnapshotsByOrdinals(newOrdinal, oldOrdinal);

        var rootStripped = RemoveRootPrefix(compared);

        SaveOutput(rootStripped);

        return ExitCode.Success;
    }

    #region ctor helpers

    private void ConfigureIncludeSwitches(CommandLineOptions options)
    {
        var fs = options.CompareFileStates;
        _includeAdded = (fs & CompareFileStates.Added) == CompareFileStates.Added;
        _includeModified = (fs & CompareFileStates.Modified) == CompareFileStates.Modified;
        _includeUnmodified = (fs & CompareFileStates.Unmodified) == CompareFileStates.Unmodified;
        _includeDeleted = (fs & CompareFileStates.Deleted) == CompareFileStates.Deleted;
    }

    #endregion

    private (int, int?) GetSnapshotOrdinals(ICollection<IStoreEntry> entries)
    {
        if (entries.Count == 0)
        {
            throw new SnapshotNotFoundException("Store is empty.");
        }

        if (_options.CompareMode == CompareMode.SpecifiedSnapshots)
        {
            int newSpecifiedOrdinal = _options.NewCompareSnapshot!.Value;
            int oldSpecifiedOrdinal = _options.OldCompareSnapshot!.Value;

            return (newSpecifiedOrdinal, oldSpecifiedOrdinal);
        }

        int newOrdinal = entries.Count - 1;
        int? oldOrdinal = entries.Count == 1
                                ? null
                                : _options.CompareMode switch
                                {
                                    CompareMode.LastPrevious => entries.Count - 2,
                                    CompareMode.LastFirst => 0,
                                    _ => throw new NotSupportedException()
                                };

        return (newOrdinal, oldOrdinal);
    }

    private IEnumerable<string> CompareSnapshotsByOrdinals(int newOrdinal, int? oldOrdinal)
    {
        var newSnapshot = _store.GetByOrdinal(newOrdinal)
                          ?? throw new SnapshotNotFoundException(newOrdinal);

        if (!oldOrdinal.HasValue)
        {
            var comparedWithEmpty = CompareWithEmptySnapshot(newSnapshot);

            _logger.Debug("Compared snapshot #{0} with empty snapshot.", newOrdinal);
            LogTotals();

            return comparedWithEmpty;
        }

        var oldSnapshot = _store.GetByOrdinal(oldOrdinal.Value)
                          ?? throw new SnapshotNotFoundException(oldOrdinal.Value);

        var compared = Compare(newSnapshot, oldSnapshot);

        _logger.Debug("Compared snapshots #{0} and #{1}", newOrdinal, oldOrdinal);
        LogTotals();

        return compared;
    }
    
    #region Compare
    
    private IEnumerable<string> CompareWithEmptySnapshot(IStoreRecord newSnapshot)
    {
        var paths = _includeAdded
            ? newSnapshot.Files.Keys
            : Enumerable.Empty<string>();

        var result = paths.ToList();
        result.Sort();

        _numAdded = result.Count;

        return result;
    }

    private IEnumerable<string> Compare(IStoreRecord newSnapshot, IStoreRecord oldSnapshot)
    {
        var oldSnapFiles = oldSnapshot.Files.ToDictionary(x => x.Key, x => new SeenState(x.Value, false));
        var newSnapFiles = newSnapshot.Files;

        var result = new List<string>();

        static void doNothing(string x) { }

        Action<string> addAdded = _includeAdded ? x => { result.Add(x); _numAdded++; } : doNothing;
        Action<string> addModified = _includeModified ? x => { result.Add(x); _numModified++; } : doNothing;
        Action<string> addUnmodified = _includeUnmodified ? x => { result.Add(x); _numUnmodified++; } : doNothing;
        Action<string> addDeleted = _includeDeleted ? x => { result.Add(x); _numDeleted++; } : doNothing;

        foreach (var newSnapFile in newSnapFiles)
        {
            string newFilePath = newSnapFile.Key;

            var oldState = oldSnapFiles.GetValueOrDefault(newFilePath);
            if (oldState == null)
            {
                addAdded(newFilePath);
                continue;
            }

            oldState.Seen = true;

            bool same = newSnapFile.Value.Equals(oldState.FileStatus);
            if (same)
            {
                addUnmodified(newFilePath);
            }
            else
            {
                addModified(newFilePath);
            }
        }

        foreach (var oldSnapFile in oldSnapFiles)
        {
            bool seen = oldSnapFile.Value.Seen;
            if (!seen)
            {
                string oldFilePath = oldSnapFile.Key;
                addDeleted(oldFilePath);
            }
        }

        result.Sort();
        return result;
    }

    #endregion

    private IEnumerable<string> RemoveRootPrefix(IEnumerable<string> lines)
    {
        if (!lines.Any() || string.IsNullOrWhiteSpace(_options.CompareRelativePath))
        {
            return lines;
        }

        string root = _options.CompareRelativePath!;
        if (!Path.EndsInDirectorySeparator(root))
        {
            root += new string(Path.DirectorySeparatorChar, 1);
        }
        int rootLength = root.Length;

        var result = lines.Select(x => x.StartsWith(root, StringComparison.Ordinal) ? x.Substring(rootLength) : x);
        return result;
    }

    private void SaveOutput(IEnumerable<string> lines)
    {
        File.WriteAllLines(_options.CompareOutputFile!, lines, Encoding.UTF8);
    }

    private void LogTotals()
    {
        if (_includeAdded)
        {
            _logger.Debug("Files added: {0}", _numAdded);
        }

        if (_includeModified)
        {
            _logger.Debug("Files modified: {0}", _numModified);
        }

        if (_includeUnmodified)
        {
            _logger.Debug("Files unmodified: {0}", _numUnmodified);
        }

        if (_includeDeleted)
        {
            _logger.Debug("Files deleted: {0}", _numDeleted);
        }
    }

    private class SeenState
    {
        public SeenState(FileStatus fileStatus, bool seen)
        {
            FileStatus = fileStatus;
            Seen = seen;
        }

        public FileStatus FileStatus { get; }

        public bool Seen { get; set; }
    }
}
