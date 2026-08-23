namespace LsChanged.Logging;

internal class ConsoleLogger(bool verbose) : ILogger
{
    private enum LogLevel
    {
        Trace = 1,
        Debug = 2,
        Info = 3,
        Warning = 4,
        Error = 5
    }

    private readonly LogLevel _logLevel = verbose ? LogLevel.Debug : LogLevel.Info;

    public void Debug(string message) =>
        Print(LogLevel.Debug, message);
    public void Debug<T0>(string format, T0 arg0) =>
        Print(LogLevel.Debug, format, arg0);
    public void Debug<T0, T1>(string format, T0 arg0, T1 arg1) =>
        Print(LogLevel.Debug, format, arg0, arg1);
    public void Debug<T0, T1>(string format, T0 arg0, T1 arg1, params object[] args) =>
        Print(LogLevel.Debug, format, arg0, arg1, args);

    public void Info(string message) =>
        Print(LogLevel.Info, message);
    public void Info<T0>(string format, T0 arg0) =>
        Print(LogLevel.Info, format, arg0);
    public void Info<T0, T1>(string format, T0 arg0, T1 arg1) =>
        Print(LogLevel.Info, format, arg0, arg1);
    public void Info<T0, T1>(string format, T0 arg0, T1 arg1, params object[] args) =>
        Print(LogLevel.Info, format, arg0, arg1, args);

    public void Error(string message) =>
        Print(LogLevel.Error, message);
    public void Error<T0>(string format, T0 arg0) =>
        Print(LogLevel.Error, format, arg0);
    public void Error<T0, T1>(string format, T0 arg0, T1 arg1) =>
        Print(LogLevel.Error, format, arg0, arg1);
    public void Error<T0, T1>(string format, T0 arg0, T1 arg1, params object[] args) =>
        Print(LogLevel.Error, format, arg0, arg1, args);

    #region Print

    private static TextWriter GetWriter(LogLevel level)
    {
        var writer = level switch
        {
            LogLevel.Error => Console.Error,
            _ => Console.Out
        };
        return writer;
    }

    private void Print(LogLevel level, string message)
    {
        if (level < _logLevel)
        {
            return;
        }

        var writer = GetWriter(level);
        writer.WriteLine(message);
    }

    private void Print<T0>(LogLevel level, string format, T0 arg0)
    {
        if (level < _logLevel)
        {
            return;
        }

        var writer = GetWriter(level);
        string message = string.Format(format, arg0);
        writer.WriteLine(message);
    }

    private void Print<T0, T1>(LogLevel level, string format, T0 arg0, T1 arg1)
    {
        if (level < _logLevel)
        {
            return;
        }

        var writer = GetWriter(level);
        string message = string.Format(format, arg0, arg1);
        writer.WriteLine(message);
    }

    private void Print<T0, T1>(LogLevel level, string format, T0 arg0, T1 arg1, object?[] args)
    {
        if (level < _logLevel)
        {
            return;
        }

        var writer = GetWriter(level);

        object?[] arguments = new object?[2 + args.Length];
        arguments[0] = arg0;
        arguments[1] = arg1;
        Array.Copy(args, 0, arguments, 2, args.Length);

        string message = string.Format(format, arguments);
        writer.WriteLine(message);
    }

    #endregion
}
