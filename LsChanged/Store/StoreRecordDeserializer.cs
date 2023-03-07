using LsChanged.Exceptions;
using LsChanged.Store.Abstractions;
using LsChanged.Store.Serialization;
using System.Text.Json;

namespace LsChanged.Store;

internal class StoreRecordDeserializer : IStoreRecordDeserializer
{
    public IStoreRecord Deserialize(string serialized)
    {
        ArgumentException.ThrowIfNullOrEmpty(serialized);

        try
        {
            var result = JsonSerializer.Deserialize(serialized, StoreRecordSerializerContext.Default.StoreRecord)
                         ?? throw new StoreEntrySerializationException("Unable to deserialize Store Record.");
            return result;
        }
        catch (Exception exception)
        {
            throw new StoreEntrySerializationException("Unable to deserialize Store Record.", exception);
        }
    }
}
