namespace LsChanged.Exceptions;

internal class InvalidIgnoreFileException(string ignoreFilePath, Exception innerException)
    : FatalExitException(
        $"Ignore file '{ignoreFilePath}' is invalid: {innerException.Message}",
        ProgramRunner.ExitCode.InvalidIgnoreFile,
        innerException)
{
}
