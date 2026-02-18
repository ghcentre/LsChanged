using LsChanged.Logging;
using LsChanged.Store.Abstractions;

namespace LsChanged.ProgramRunner;

internal class ListStrategy(ILogger logger, IStore store) : IRunnerStrategy
{
    public int Run()
    {
        var entries = store.ListAll().ToList();

        logger.Debug("     # Id");
        logger.Debug("------ --------------------------------------------------");

        for (int i = 0; i < entries.Count; i++)
        {
            logger.Info("{0,6} {1}", i, entries[i].Id);
        }

        logger.Debug(Environment.NewLine + "Total: {0}", entries.Count);

        return ExitCode.Success;
    }
}
