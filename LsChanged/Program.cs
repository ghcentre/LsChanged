using LsChanged.CommandLine;
using LsChanged.Settings;
using LsChanged.Store;
using LsChanged.Store.Abstractions;

namespace LsChanged;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            var optionsRuleProvider = new OptionRuleProvider();
            var rules = OptionRuleProvider.Rules;
            
            var commandLineParser = new CommandLineParser(rules);
            
            var options = commandLineParser.Parse(args);
            options.Validate();

            var store = InitializeStore(options);

            if (options.Command == Command.Scan)
            {
                Scan(options, store);
            }

            return ExitCode.Success;
        }
        catch (FatalExitException fatalExitException)
        {
            Console.Error.WriteLine(fatalExitException.Message);
            return fatalExitException.ExitCode;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Fatal: {exception}");
            return ExitCode.GenericError;
        }
    }

    private static IStore InitializeStore(CommandLineOptions options)
    {
        var deserializer = new StoreRecordDeserializer();
        var reader = new StoreRecordReader(deserializer);

        var serializer = new StoreRecordSerializer();
        var writer = new StoreRecordWriter(serializer);

        var ctp = new CurrentTimeProvider();

        var store = new Store.Store(options.StorePath!, ctp, reader, writer);

        return store;
    }

    private static void Scan(CommandLineOptions options, IStore store)
    {
        var collectorSettings = new FileInfoCollectorSettings(options.FollowSymlinks);
        var collector = new FileInfoCollector(collectorSettings);

        var entries = collector.Collect(options.ScanPath!);

        var storeRecordFactory = new StoreRecordFactory();
        var storeRecord = storeRecordFactory.CreateFromFiles(DateTime.UtcNow, entries);

        store.Add(storeRecord);
    }
}