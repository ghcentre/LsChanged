using LsChanged.Collector;

namespace LsChanged.Store.Abstractions;

internal interface IStoreRecord
{
    int Version { get; }

    DateTime CreatedAtUtc { get; }

    IReadOnlyDictionary<string, FileStatus> Files { get; }
}
