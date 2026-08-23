using LsChanged.CommandLine;
using LsChanged.Exceptions;
using LsChanged.Logging;
using System.Reflection;
using System.Text;

namespace LsChanged.ProgramRunner;

internal class NewIgnoreStrategy(ILogger logger, CommandLineOptions options)
    : IRunnerStrategy
{
    public int Run()
    {
        string filePath = options.IgnoreFilePath!;
        if (File.Exists(filePath))
        {
            throw new FileAlreadyExistsException(filePath);
        }

        var assembly = Assembly.GetExecutingAssembly();
        const string resourceName = "lschanged.IgnoreFile.txt";

        using var stream = assembly.GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException($"Could not find resource '{resourceName}'.");

        using var reader = new StreamReader(stream);

        string content = reader.ReadToEnd();
        string[] lines = content.Replace("\r", string.Empty).Split('\n');

        var encoding = new UTF8Encoding(false);

        var fileStreamOptions = new FileStreamOptions()
        {
            Mode = FileMode.CreateNew,
            Access = FileAccess.Write
        };
        using var writer = new StreamWriter(filePath, encoding, fileStreamOptions);

        foreach (string line in lines)
        {
            writer.WriteLine(line);
        }

        logger.Debug($"New ignore file created at '{filePath}'.");
        return ExitCode.Success;
    }
}
