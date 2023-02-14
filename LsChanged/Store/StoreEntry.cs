using LsChanged.Store.Abstractions;

namespace LsChanged.Store;

internal class StoreEntry : IStoreEntry
{
    public StoreEntry(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        Id = id;
    }

    public string Id { get; }
}