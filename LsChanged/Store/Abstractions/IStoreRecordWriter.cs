namespace LsChanged.Store.Abstractions;

internal interface IStoreRecordWriter
{
    IStoreEntry Write(IStoreEntry entry, IStoreRecord storeRecord);
}
