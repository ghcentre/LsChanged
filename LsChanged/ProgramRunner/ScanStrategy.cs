using LsChanged.Collector;
using LsChanged.CommandLine;
using LsChanged.Logging;
using LsChanged.Store.Abstractions;

namespace LsChanged.ProgramRunner;

internal class ScanStrategy : IRunnerStrategy
{
    private readonly ILogger _logger;
    private readonly CommandLineOptions _options;
    private readonly IStore _store;
    private readonly Func<CommandLineOptions, IFileInfoCollector> _collectorFactory;
    private readonly IStoreRecordFactory _storeRecordFactory;
    private readonly ICurrentTimeProvider _currentTimeProvider;

    public ScanStrategy(ILogger logger,
                        CommandLineOptions options,
                        IStore store,
                        Func<CommandLineOptions, IFileInfoCollector> collectorFactory,
                        IStoreRecordFactory storeRecordFactory,
                        ICurrentTimeProvider currentTimeProvider)
    {
        _logger = logger;
        _options = options;
        _store = store;
        _collectorFactory = collectorFactory;
        _storeRecordFactory = storeRecordFactory;
        _currentTimeProvider = currentTimeProvider;
    }

    public int Run()
    {
        var collector = _collectorFactory(_options);
        var entries = collector.Collect(_options.ScanPath!);
        
        var now = _currentTimeProvider.CurrentTime;
        var storeRecord = _storeRecordFactory.CreateFromFiles(now, entries);

        _store.Add(storeRecord);

        return ExitCode.Success;
    }
}
