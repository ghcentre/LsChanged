namespace LsChanged.Exceptions;

internal sealed class IgnoreFileNotFoundException(string ignoreFilePath)
    : FatalExitException(
        $"Ignore file '{ignoreFilePath}' does not exist.",
        ProgramRunner.ExitCode.IgnoreFileNotFound)
{
}
