namespace LsChanged.Exceptions;

internal sealed class CommandLineParseException : FatalExitException
{
    public CommandLineParseException(string message)
        : base(message, ProgramRunner.ExitCode.InvalidCommandLine)
    {
    }

    public CommandLineParseException(string message, Exception innerException)
        : base(message, ProgramRunner.ExitCode.InvalidCommandLine, innerException)
    {
    }
}
