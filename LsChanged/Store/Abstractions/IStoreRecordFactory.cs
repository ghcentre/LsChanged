using LsChanged.Collector;

namespace LsChanged.Store.Abstractions
{
    internal interface IStoreRecordFactory
    {
        IStoreRecord CreateFromFiles(DateTime createdAt, IReadOnlyDictionary<string, FileStatus> files);
    }
}