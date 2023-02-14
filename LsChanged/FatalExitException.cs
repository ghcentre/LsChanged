namespace LsChanged;

internal sealed class FatalExitException : Exception
{
    public FatalExitException(string message, int exitCode)
        : base(message)
    {
        ExitCode = exitCode;
    }

    public FatalExitException(string message, int exitCode, Exception innerException)
        : base(message, innerException)
    {
        ExitCode = exitCode;
    }

    public int ExitCode { get; }
}
