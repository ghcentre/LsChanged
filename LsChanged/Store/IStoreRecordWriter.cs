namespace LsChanged.Store;

internal interface IStoreRecordWriter
{
    IStoreEntry Write(IStoreEntry entry, IStoreRecord storeRecord);
}
