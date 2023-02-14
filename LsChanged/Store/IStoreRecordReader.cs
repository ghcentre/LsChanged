namespace LsChanged.Store;

internal interface IStoreRecordReader
{
    IStoreRecord Read(IStoreEntry entry);
}
