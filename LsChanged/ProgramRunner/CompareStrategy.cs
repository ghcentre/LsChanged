using LsChanged.CommandLine;
using LsChanged.Logging;
using LsChanged.Store.Abstractions;

namespace LsChanged.ProgramRunner;

internal class CompareStrategy : IRunnerStrategy
{
    private readonly ILogger _logger;
    private readonly CommandLineOptions _options;
    private readonly IStore _store;

    public CompareStrategy(ILogger logger, CommandLineOptions options, IStore store)
    {
        _logger = logger;
        _options = options;
        _store = store;
    }

    public int Run()
    {
        Console.WriteLine($"Compare mode: {_options.CompareMode}");
        Console.WriteLine($"Output file: {_options.CompareOutputFile}");
        Console.WriteLine($"Snapshots: {_options.NewCompareSnapshot},{_options.OldCompareSnapshot}");
        Console.WriteLine($"FileStates: {_options.CompareFileStates}");
        Console.WriteLine($"RelativePath: {_options.CompareRelativePath}");

        if (_options.CompareMode == LsChanged.Compare.CompareMode.SpecifiedSnapshots)
        {
            var newSnapshot = _store.GetByOrdinal(_options.NewCompareSnapshot!.Value);
            var oldSnapshot = _store.GetByOrdinal(_options.OldCompareSnapshot!.Value);
        }

        throw new NotImplementedException();
    }
}
