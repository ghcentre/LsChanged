using LsChanged.Logging;
using LsChanged.Store.Abstractions;

namespace LsChanged.ProgramRunner;

internal class ListStrategy : IRunnerStrategy
{
    private readonly ILogger _logger;
    private readonly IStore _store;

    public ListStrategy(ILogger logger, IStore store)
    {
        _logger = logger;
        _store = store;
    }

    public int Run()
    {
        var entries = _store.ListAll().ToList();

        _logger.Debug("     # Id");
        _logger.Debug("------ --------------------------------------------------");

        for (int i = 0; i < entries.Count; i++)
        {
            _logger.Info("{0,6} {1}", i, entries[i].Id);
        }

        _logger.Debug(Environment.NewLine + "Total: {0}", entries.Count);

        return ExitCode.Success;
    }
}
