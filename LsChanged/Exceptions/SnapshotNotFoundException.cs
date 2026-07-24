namespace LsChanged.Exceptions;

internal sealed class SnapshotNotFoundException : FatalExitException
{
    public SnapshotNotFoundException(int ordinal)
        : base($"Snapshot #{ordinal} does not exist.", ProgramRunner.ExitCode.SnapshotNotFound)
    {
    }

    public SnapshotNotFoundException(string message)
        : base(message, ProgramRunner.ExitCode.SnapshotNotFound)
    {
    }
}
