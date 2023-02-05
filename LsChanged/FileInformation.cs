namespace LsChanged;

internal class FileInformation
{
    public FileInformation(long size, DateTimeOffset lastWrite, int attributes, int mode)
    {
        Size = size;
        LastWrite = lastWrite;
        Attributes = attributes;
        Mode = mode;
    }

    public long Size { get; }
    public DateTimeOffset LastWrite { get; }
    public int Attributes { get; }
    public int Mode { get; }
}
