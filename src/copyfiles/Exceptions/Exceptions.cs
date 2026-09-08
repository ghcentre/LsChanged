namespace CopyFiles.Exceptions;

internal sealed class DirectoryDoesNotExistException(string path)
    : FatalExitException(
        $"The specified directory '{path}' does not exist.",
        ExitCodes.DirectoryDoesNotExist);

internal sealed class DirectoryNotReadableException(string path, Exception innerException)
    : FatalExitException(
        $"The specified directory '{path}' is not readable.",
        ExitCodes.DirectoryNotReadable,
        innerException);

internal sealed class DirectoryNotWritableException(string path, Exception innerException)
    : FatalExitException(
        $"The specified directory '{path}' is not writable.",
        ExitCodes.DirectoryNotWritable,
        innerException);

internal sealed class IOErrorException(string path, Exception innerException)
    : FatalExitException(
        $"An I/O error occurred while accessing the file or directory '{path}'.",
        ExitCodes.IOError,
        innerException);

internal sealed class FileDoesNotExistException(string path)
    : FatalExitException(
        $"The specified file '{path}' does not exist.",
        ExitCodes.FileDoesNotExist);

internal sealed class FileNotReadableException(string path, Exception innerException)
    : FatalExitException(
        $"The specified file '{path}' is not readable.",
        ExitCodes.FileNotReadable,
        innerException);

internal sealed class InvalidRelativeFilePathException(string path)
    : FatalExitException(
        $"The specified relative file path '{path}' is invalid.",
        ExitCodes.InvalidFilePath);
