namespace LsChanged.Store.Abstractions;

internal interface IStoreRecordDeserializer
{
    IStoreRecord Deserialize(byte[] serialized);
}
