namespace LsChanged.Collector;

internal sealed class FileStatus(long size, DateTime lastWriteUtc, int attributes, int mode)
{
    public long Size { get; } = size;

    public DateTime LastWriteUtc { get; } = lastWriteUtc;

    public int Attributes { get; } = attributes;

    public int Mode { get; } = mode;

    public override bool Equals(object? obj)
    {
        return obj is FileStatus status &&
               Size == status.Size &&
               LastWriteUtc == status.LastWriteUtc &&
               Attributes == status.Attributes &&
               Mode == status.Mode;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Size, LastWriteUtc, Attributes, Mode);
    }
}
