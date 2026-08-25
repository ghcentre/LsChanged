namespace LsChanged.Exceptions;

internal class IgnoreFileNotFoundException(string ignoreFilePath)
    : FatalExitException(
        $"Ignore file '{ignoreFilePath}' does not exist.",
        ProgramRunner.ExitCode.IgnoreFileNotFound)
{
}
