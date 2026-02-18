namespace LsChanged.Exceptions;

internal class SnapshotNotFoundException : Exception
{
    public SnapshotNotFoundException(int ordinal)
        : base($"Snapshot #{ordinal} does not exist.")
    {
    }

    public SnapshotNotFoundException(string message)
        : base(message)
    {
    }
}
