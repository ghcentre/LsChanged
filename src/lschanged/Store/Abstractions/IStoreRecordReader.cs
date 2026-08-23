namespace LsChanged.Store.Abstractions;

internal interface IStoreRecordReader
{
    IStoreRecord Read(IStoreEntry entry);
}
