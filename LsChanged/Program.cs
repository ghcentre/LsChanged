using LsChanged.Collector;
using LsChanged.CommandLine;
using LsChanged.Environment;
using LsChanged.Exceptions;
using LsChanged.Logging;
using LsChanged.Store;
using LsChanged.Store.Abstractions;
using System.Reflection;

namespace LsChanged;

internal static class Program
{
    private static int Main(string[] args)
    {
        var logger = CreateLogger(null);

        try
        {
            var commandLineParser = new CommandLineParser(OptionRuleProvider.Rules, o => o.SetCommand(Command.Help));
            var options = commandLineParser.Parse(args);

            if (options.Command == Command.Help)
            {
                Help(logger);
                return ExitCode.HelpDisplayed;
            }

            options.Validate();

            logger = CreateLogger(options);

            var store = InitializeStore(options);

            switch (options.Command)
            {
                case Command.Scan:
                    Scan(logger, options, store);
                    break;

                case Command.Compare:
                    throw new NotImplementedException();
                    break;

                case Command.List:
                    throw new NotImplementedException();
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
            logger.Error(commandLineParseException.Message);
            return ExitCode.InvalidCommandLine;
        }
        catch (FatalExitException fatalExitException)
        {
            logger.Error(fatalExitException.Message);
            return fatalExitException.ExitCode;
        }
        catch (Exception exception)
        {
            logger.Error("Fatal: {0}", exception);
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

    private static void Scan(ILogger logger, CommandLineOptions options, IStore store)
    {
        var collector = new FileInfoCollector(logger, options.FollowSymlinks);

        var entries = collector.Collect(options.ScanPath!);

        var storeRecordFactory = new StoreRecordFactory();
        var storeRecord = storeRecordFactory.CreateFromFiles(DateTime.UtcNow, entries);

        store.Add(storeRecord);
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

    private static ILogger CreateLogger(CommandLineOptions? options)
    {
        bool verbose = options?.Verbose ?? false;
        var logger = new ConsoleLogger(verbose);
        return logger;
    }
}