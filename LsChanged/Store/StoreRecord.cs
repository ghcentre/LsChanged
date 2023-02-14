namespace LsChanged.Store;

internal class StoreRecord : IStoreRecord
{
    private const int _version1 = 1;

    public StoreRecord(DateTime createdAtUtc, IReadOnlyDictionary<string, FileStatus> files)
    {
        CreatedAtUtc = createdAtUtc.ToUniversalTime();

        ArgumentNullException.ThrowIfNull(files);
        Files = files;
    }

    public int Version { get; } = _version1;

    public DateTime CreatedAtUtc { get; }

    public IReadOnlyDictionary<string, FileStatus> Files { get; }
}
