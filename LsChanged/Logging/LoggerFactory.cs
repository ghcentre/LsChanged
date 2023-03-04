using LsChanged.CommandLine;

namespace LsChanged.Logging;

internal class LoggerFactory : ILoggerFactory
{
    private readonly Func<CommandLineOptions> _commandLineOptionsFactory;

    public LoggerFactory(Func<CommandLineOptions> commandLineOptionsFactory)
    {
        ArgumentNullException.ThrowIfNull(commandLineOptionsFactory);
        _commandLineOptionsFactory = commandLineOptionsFactory;
    }

    public ILogger CreateLogger()
    {
        var options = _commandLineOptionsFactory();
        var instance = new ConsoleLogger(options.Verbose);
        return instance;
    }

    public ILogger CreateBootstrapLogger()
    {
        var instance = new ConsoleLogger(false);
        return instance;
    }
}
