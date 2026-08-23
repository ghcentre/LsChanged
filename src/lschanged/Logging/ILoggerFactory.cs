namespace LsChanged.Logging;

internal interface ILoggerFactory
{
    ILogger CreateLogger();

    ILogger CreateBootstrapLogger();
}
