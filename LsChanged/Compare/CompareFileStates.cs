namespace LsChanged.Compare;

[Flags]
internal enum CompareFileStates
{
    None = 0x0,
    Added = 0x1,
    Modified = 0x2,
    Unmodified = 0x4,
    Deleted = 0x8
}
