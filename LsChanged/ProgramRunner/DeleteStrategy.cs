using LsChanged.CommandLine;
using LsChanged.Exceptions;
using LsChanged.Logging;
using LsChanged.Store.Abstractions;

namespace LsChanged.ProgramRunner;

internal class DeleteStrategy : IRunnerStrategy
{
    private readonly ILogger _logger;
    private readonly CommandLineOptions _options;
    private readonly IStore _store;

    public DeleteStrategy(ILogger logger, CommandLineOptions options, IStore store)
    {
        _logger = logger;
        _options = options;
        _store = store;
    }

    public int Run()
    {
        var entries = _store.ListAll().ToList();

        int ordinal = _options.SnaphotToDelete ?? entries.Count - 1;
        if (ordinal < 0 || ordinal >= entries.Count)
        {
            throw new SnapshotNotFoundException(ordinal);
        }

        var entry = entries[ordinal];
        bool success = _store.Delete(entry);
        _logger.Debug(
            success
                ? $"Snapshot #{ordinal} deleted successfully."
                : $"Snapshot #{ordinal} could not be deleted.");

        return success ? ExitCode.Success : ExitCode.SnapshotNotFound;
    }
}
