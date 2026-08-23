using LsChanged.ProgramRunner;
using LsChanged.Logging;
using System.Reflection;

namespace LsChanged.ProgramRunner;

internal class HelpStrategy(ILogger logger, IHelpTextFilter helpTextFilter) : IRunnerStrategy
{
    public int Run()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;

        logger.Info("LsChanged {0}", version);
        logger.Info("Copyright (c) 2023-2026 George Harder's Centre (https://ghcentre.com)");
        logger.Info("Creates filesystem metadata snapshots. Compares snapshots and lists files changed.");
        logger.Info(string.Empty);

        const string resourceName = "lschanged.CommandLineReference.txt";

        using var stream = assembly.GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException($"Could not find resource '{resourceName}'.");

        using var reader = new StreamReader(stream);

        string content = reader.ReadToEnd();
        var lines = helpTextFilter.Filter(content);

        foreach (string line in lines)
        {
            logger.Info(line);
        }

        return ExitCode.HelpDisplayed;
    }
}
