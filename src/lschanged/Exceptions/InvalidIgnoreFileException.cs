namespace LsChanged.Exceptions;

internal sealed class InvalidIgnoreFileException(string ignoreFilePath, Exception innerException)
    : FatalExitException(
        $"Ignore file '{ignoreFilePath}' is invalid: {innerException.Message}",
        ProgramRunner.ExitCode.InvalidIgnoreFile,
        innerException)
{
}
