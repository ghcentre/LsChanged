namespace LsChanged.Store;

internal interface IStoreRecordDeserializer
{
    IStoreRecord Deserialize(string serialized);
}
