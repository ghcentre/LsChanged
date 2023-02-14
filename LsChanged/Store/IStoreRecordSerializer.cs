namespace LsChanged.Store;

internal interface IStoreRecordSerializer
{
    string Serialize(IStoreRecord storeRecord);
}
