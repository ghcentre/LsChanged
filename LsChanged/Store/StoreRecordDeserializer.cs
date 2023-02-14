using LsChanged.Exceptions;
using System.Text.Json;

namespace LsChanged.Store;

internal class StoreRecordDeserializer : IStoreRecordDeserializer
{
    public IStoreRecord Deserialize(string serialized)
    {
        ArgumentException.ThrowIfNullOrEmpty(serialized);

        try
        {
            var result = JsonSerializer.Deserialize<StoreRecord>(serialized)
                         ?? throw new StoreEntrySerializationException("Unable to deserialize Store Record.");
            return result;
        }
        catch (Exception exception)
        {
            throw new StoreEntrySerializationException("Unable to deserialize Store Record.", exception);
        }
    }
}
