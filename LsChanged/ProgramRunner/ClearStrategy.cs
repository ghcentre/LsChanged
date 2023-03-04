using LsChanged.Logging;
using LsChanged.Store.Abstractions;

namespace LsChanged.ProgramRunner;

internal class ClearStrategy : IRunnerStrategy
{
    private readonly ILogger _logger;
    private readonly IStore _store;

    public ClearStrategy(ILogger logger, IStore store)
    {
        _logger = logger;
        _store = store;
    }

    public int Run()
    {
        _store.Clear();
        _logger.Debug("Store cleared.");

        return ExitCode.Success;
    }
}
