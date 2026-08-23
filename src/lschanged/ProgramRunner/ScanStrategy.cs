using LsChanged.Collector;
using LsChanged.CommandLine;
using LsChanged.Store.Abstractions;

namespace LsChanged.ProgramRunner;

internal class ScanStrategy(CommandLineOptions options,
                            IStore store,
                            Func<CommandLineOptions, IFileInfoCollector> collectorFactory,
                            IStoreRecordFactory storeRecordFactory,
                            ICurrentTimeProvider currentTimeProvider)
    : IRunnerStrategy
{
    public int Run()
    {
        var collector = collectorFactory(options);
        var entries = collector.Collect(options.ScanPath!);

        var now = currentTimeProvider.CurrentTime;
        var storeRecord = storeRecordFactory.CreateFromFiles(now, entries);

        store.Add(storeRecord);

        return ExitCode.Success;
    }
}
