namespace LsChanged.Exceptions;

internal sealed class FileAlreadyExistsException(string filePath)
    : FatalExitException($"File already exists: {filePath}", ProgramRunner.ExitCode.IgnoreFileAlreadyExists)
{
}
