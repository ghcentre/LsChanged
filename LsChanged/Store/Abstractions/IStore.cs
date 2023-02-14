namespace LsChanged.Store.Abstractions;

internal interface IStore
{
    IEnumerable<IStoreEntry> ListAll();

    IStoreRecord Get(IStoreEntry storeEntry);

    IStoreEntry Add(IStoreRecord storeRecord);

    bool Remove(IStoreEntry storeEntry);

    void Clear();
}
