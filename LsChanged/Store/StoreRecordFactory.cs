using LsChanged.Collector;
using LsChanged.Store.Abstractions;

namespace LsChanged.Store;

internal class StoreRecordFactory
{
    public IStoreRecord CreateFromFiles(DateTime createdAt, IReadOnlyDictionary<string, FileStatus> files)
    {
        ArgumentNullException.ThrowIfNull(files);

        var instance = new StoreRecord(createdAt, files);
        return instance;
    }
}
