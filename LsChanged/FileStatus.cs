namespace LsChanged;

internal class FileStatus
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
}
