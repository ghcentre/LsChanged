using LsChanged.Logging;
using LsChanged.Store.Abstractions;

namespace LsChanged.ProgramRunner;

internal class ClearStrategy(ILogger logger, IStore store) : IRunnerStrategy
{
    public int Run()
    {
        store.Clear();
        logger.Debug("Store cleared.");

        return ExitCode.Success;
    }
}
