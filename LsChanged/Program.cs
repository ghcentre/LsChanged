using LsChanged.Collector;
using LsChanged.CommandLine;
using LsChanged.Environment;
using LsChanged.Exceptions;
using LsChanged.Store;
using LsChanged.Store.Abstractions;
using System.Reflection;

namespace LsChanged;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            var commandLineParser = new CommandLineParser(OptionRuleProvider.Rules, o => o.SetCommand(Command.Help));
            
            var options = commandLineParser.Parse(args);

            if (options.Command == Command.Help)
            {
                Help();
                return ExitCode.HelpDisplayed;
            }

            options.Validate();

            var store = InitializeStore(options);

            switch (options.Command)
            {
                case Command.Scan:
                    Scan(options, store);
                    break;

                case Command.Compare:
                    break;

                case Command.List:
                    break;

                case Command.Delete:
                    break;

                case Command.Clear:
                    break;

                default:
                    throw new NotSupportedException();
            }

            return ExitCode.Success;
        }
        catch (CommandLineParseException commandLineParseException)
        {
            Console.Error.WriteLine(commandLineParseException.Message);
            return ExitCode.InvalidCommandLine;
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
        var collector = new FileInfoCollector(options.FollowSymlinks);

        var entries = collector.Collect(options.ScanPath!);

        var storeRecordFactory = new StoreRecordFactory();
        var storeRecord = storeRecordFactory.CreateFromFiles(DateTime.UtcNow, entries);

        store.Add(storeRecord);
    }

    private static void Help()
    {
        const string resourceName = "LsChanged.CommandLineReference.txt";
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException($"Could not find resource '{resourceName}'.");
        using var reader = new StreamReader(stream);
        
        string content = reader.ReadToEnd();

        Console.Out.WriteLine(content);
    }
}