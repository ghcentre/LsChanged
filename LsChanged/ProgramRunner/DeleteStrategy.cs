using LsChanged.CommandLine;
using LsChanged.Exceptions;
using LsChanged.Logging;
using LsChanged.Store.Abstractions;

namespace LsChanged.ProgramRunner;

internal class DeleteStrategy(ILogger logger, CommandLineOptions options, IStore store) : IRunnerStrategy
{
    public int Run()
    {
        var entries = store.ListAll().ToList();

        int ordinal = options.SnaphotToDelete ?? entries.Count - 1;
        if (ordinal < 0 || ordinal >= entries.Count)
        {
            throw new SnapshotNotFoundException(ordinal);
        }

        var entry = entries[ordinal];
        bool success = store.Delete(entry);
        logger.Debug(
            success
                ? $"Snapshot #{ordinal} deleted successfully."
                : $"Snapshot #{ordinal} could not be deleted.");

        return success ? ExitCode.Success : ExitCode.SnapshotNotFound;
    }
}
