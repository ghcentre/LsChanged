namespace LsChanged.Logging;

internal interface ILogger
{
    void Debug(string message);
    void Debug<T0>(string format, T0 arg0);
    void Debug<T0, T1>(string format, T0 arg0, T1 arg1);
    void Debug<T0, T1>(string format, T0 arg0, T1 arg1, params object[] args);

    void Info(string message);
    void Info<T0>(string format, T0 arg0);
    void Info<T0, T1>(string format, T0 arg0, T1 arg1);
    void Info<T0, T1>(string format, T0 arg0, T1 arg1, params object[] args);

    void Error(string message);
    void Error<T0>(string format, T0 arg0);
    void Error<T0, T1>(string format, T0 arg0, T1 arg1);
    void Error<T0, T1>(string format, T0 arg0, T1 arg1, params object[] args);
}
