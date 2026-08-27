using LsChanged.CommandLine;

namespace LsChanged.Logging;

internal sealed class LoggerFactory(Func<CommandLineOptions> commandLineOptionsFactory) : ILoggerFactory
{
    public ILogger CreateLogger()
    {
        var options = commandLineOptionsFactory();
        var instance = new ConsoleLogger(options.Verbose);
        return instance;
    }

    public ILogger CreateBootstrapLogger()
    {
        var instance = new ConsoleLogger(false);
        return instance;
    }
}
