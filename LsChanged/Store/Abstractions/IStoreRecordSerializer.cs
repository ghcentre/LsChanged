namespace LsChanged.Store.Abstractions;

internal interface IStoreRecordSerializer
{
    byte[] Serialize(IStoreRecord storeRecord);
}
