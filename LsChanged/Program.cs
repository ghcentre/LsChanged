using LsChanged.Settings;
using LsChanged.Store;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LsChanged;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length < 2)
            {
                throw new FatalExitException("Usage: lschanged path store", ExitCode.InvalidCommandLineArg);
            }

            string startingPath = GetStartingPath(args);
            string storePath = GetStorePath(args);

            var deserializer = new StoreRecordDeserializer();
            var reader = new StoreRecordReader(deserializer);

            var serializer = new StoreRecordSerializer();
            var writer = new StoreRecordWriter(serializer);

            var ctp = new CurrentTimeProvider();

            var store = new Store.Store(storePath, ctp, reader, writer);

            var collectorSettings = new FileInfoCollectorSettings(FollowSymlinksMode.PreventRecursion);
            var collector = new FileInfoCollector(collectorSettings);

            var entries = collector.Collect(startingPath);

            var storeRecordFactory = new StoreRecordFactory();
            var storeRecord = storeRecordFactory.CreateFromFiles(DateTime.UtcNow, entries);

            store.Add(storeRecord);

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


    private static string GetStartingPath(string[] args)
    {
        string? result = args[0];
        if (string.IsNullOrEmpty(result))
        {
            throw new FatalExitException("Starting path is empty.", ExitCode.InvalidPathSpecified);
        }
        return result;
    }

    private static string GetStorePath(string[] args)
    {
        string? result = args[1];
        if (string.IsNullOrEmpty(result))
        {
            throw new FatalExitException("Store is empty.", ExitCode.InvalidPathSpecified);
        }
        return result;
    }
}