namespace LsChanged.Store.Abstractions;

internal interface IStoreRecordSerializer
{
    string Serialize(IStoreRecord storeRecord);
}
