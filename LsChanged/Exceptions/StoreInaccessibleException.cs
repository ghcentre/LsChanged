namespace LsChanged.Exceptions;

internal class StoreInaccessibleException(Exception innerException)
    : FatalExitException(
        $"The store is inaccessible: {innerException.Message}",
        ProgramRunner.ExitCode.StoreUnaccessible,
        innerException)
{
}
