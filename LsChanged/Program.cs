using LsChanged.Settings;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LsChanged;

internal class Program
{
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length < 2)
            {
                throw new FatalExitException("Usage: lschanged path outfile.json", ExitCode.InvalidCommandLineArg);
            }

            string startingPath = GetStartingPath(args);
            string saveFile = GetSaveFile(args);

            var collectorSettings = new FileInfoCollectorSettings(FollowSymlinkSettings.SkipRecirsive);
            var collector = new FileInfoCollector(collectorSettings);
            var entries = collector.Collect(startingPath);

            var options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            string content = JsonSerializer.Serialize(entries, options);
            File.WriteAllText(saveFile, content);

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

    private static string GetSaveFile(string[] args)
    {
        string? result = args[1];
        if (string.IsNullOrEmpty(result))
        {
            throw new FatalExitException("Save file path is empty.", ExitCode.InvalidPathSpecified);
        }

        try
        {
            string content = Guid.NewGuid().ToString();
            File.WriteAllText(result, content);
        }
        catch (Exception exception)
        {
            throw new FatalExitException($"Could not write to file: {exception.Message}", ExitCode.WriteError, exception);
        }

        return result;
    }
}