namespace LsChanged.Collector;

internal sealed class FileStatus
{
    public FileStatus(long size, DateTime lastWriteUtc, int attributes, int mode)
    {
        Size = size;
        LastWriteUtc = lastWriteUtc;
        Attributes = attributes;
        Mode = mode;
    }

    public long Size { get; }

    public DateTime LastWriteUtc { get; }

    public int Attributes { get; }

    public int Mode { get; }

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
