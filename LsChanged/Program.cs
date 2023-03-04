using LsChanged.Collector;
using LsChanged.CommandLine;
using LsChanged.Exceptions;
using LsChanged.Logging;
using LsChanged.Store;
using LsChanged.Store.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LsChanged;

internal static class Program
{
    private static ServiceProvider CreateAndBuildServiceProvider(string[] args)
    {
        var services = new ServiceCollection();

        services.AddTransient<ILoggerFactory, LoggerFactory>();

        services.AddSingleton(
            sp =>
            {
                var commandLineParser = new CommandLineParser(OptionRuleProvider.Rules, o => o.SetCommand(Command.Help));
                var options = commandLineParser.Parse(args);
                return options;
            });
        services.AddSingleton(
            sp => new Func<CommandLineOptions>(() => sp.GetRequiredService<CommandLineOptions>()));

        services.AddTransient<IStoreRecordDeserializer, StoreRecordDeserializer>();
        services.AddTransient<IStoreRecordSerializer, StoreRecordSerializer>();
        services.AddTransient<IStoreRecordReader, StoreRecordReader>();
        services.AddTransient<IStoreRecordWriter, StoreRecordWriter>();
        services.AddTransient<ICurrentTimeProvider, CurrentTimeProvider>();

        services.AddSingleton<IStore>(
            sp =>
            {
                var options = sp.GetRequiredService<CommandLineOptions>();
                var instance = new Store.Store(options.StorePath!,
                                               sp.GetRequiredService<ICurrentTimeProvider>(),
                                               sp.GetRequiredService<IStoreRecordReader>(),
                                               sp.GetRequiredService<IStoreRecordWriter>());
                return instance;
            });

        var provider = services.BuildServiceProvider();
        return provider;
    }

    private static int Main(string[] args)
    {
        var serviceProvider = CreateAndBuildServiceProvider(args);

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var bootstrapLogger = loggerFactory.CreateBootstrapLogger();

        try
        {
            var options = serviceProvider.GetRequiredService<CommandLineOptions>();
            var logger = loggerFactory.CreateLogger();

            if (options.Command == Command.Help)
            {
                Help(bootstrapLogger);
                return ExitCode.HelpDisplayed;
            }

            options.Validate();

            var store = serviceProvider.GetRequiredService<IStore>();

            switch (options.Command)
            {
                case Command.Scan:
                    Scan(logger, options, store);
                    break;

                case Command.Compare:
                    Compare(logger, options, store);
                    break;

                case Command.List:
                    List(logger, store);
                    break;

                case Command.Delete:
                    throw new NotImplementedException();
                    break;

                case Command.Clear:
                    throw new NotImplementedException();
                    break;

                default:
                    throw new NotSupportedException();
            }

            return ExitCode.Success;
        }
        catch (CommandLineParseException commandLineParseException)
        {
            bootstrapLogger.Error(commandLineParseException.Message);
            return ExitCode.InvalidCommandLine;
        }
        catch (FatalExitException fatalExitException)
        {
            bootstrapLogger.Error(fatalExitException.Message);
            return fatalExitException.ExitCode;
        }
        catch (Exception exception)
        {
            bootstrapLogger.Error("Fatal: {0}", exception);
            return ExitCode.GenericError;
        }
    }

    private static void Scan(ILogger logger, CommandLineOptions options, IStore store)
    {
        var collector = new FileInfoCollector(logger, options.FollowSymlinksMode);

        var entries = collector.Collect(options.ScanPath!);

        var storeRecordFactory = new StoreRecordFactory();
        var storeRecord = storeRecordFactory.CreateFromFiles(DateTime.UtcNow, entries);

        store.Add(storeRecord);
    }

    private static void Compare(ILogger logger, CommandLineOptions options, IStore store)
    {
        Console.WriteLine($"Compare mode: {options.CompareMode}");
        Console.WriteLine($"Output file: {options.CompareOutputFile}");
        Console.WriteLine($"Snapshots: {options.NewCompareSnapshot},{options.OldCompareSnapshot}");
        Console.WriteLine($"FileStates: {options.CompareFileStates}");
        Console.WriteLine($"RelativePath: {options.CompareRelativePath}");

        if (options.CompareMode == LsChanged.Compare.CompareMode.SpecifiedSnapshots)
        {
            var newSnapshot = store.GetByOrdinal(options.NewCompareSnapshot!.Value);
            var oldSnapshot = store.GetByOrdinal(options.OldCompareSnapshot!.Value);
        }

        throw new NotImplementedException();
    }

    private static void List(ILogger logger, IStore store)
    {
        var entries = store.ListAll().ToList();

        logger.Debug("     # Id");
        logger.Debug("------ --------------------------------------------------");

        for (int i = 0; i < entries.Count; i++)
        {
            logger.Info("{0,6} {1}", i, entries[i].Id);
        }

        logger.Debug(Environment.NewLine + "Total: {0}", entries.Count);
    }

    private static void Help(ILogger logger)
    {
        const string resourceName = "LsChanged.CommandLineReference.txt";
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException($"Could not find resource '{resourceName}'.");
        using var reader = new StreamReader(stream);

        string content = reader.ReadToEnd();
        logger.Info(content);
    }


    internal static class ExitCode
    {
        public const int Success = 0;
        public const int InvalidCommandLine = 1;
        public const int InvalidPathSpecified = 2;
        public const int WriteError = 3;
        public const int HelpDisplayed = 254;
        public const int GenericError = 255;
    }
}