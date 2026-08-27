using LsChanged.Exceptions;
using LsChanged.Store.Abstractions;
using LsChanged.Store.Serialization;
using System.Text;
using System.Text.Json;

namespace LsChanged.Store;

internal sealed class StoreRecordDeserializer : IStoreRecordDeserializer
{
    public IStoreRecord Deserialize(byte[] serialized)
    {
        ArgumentNullException.ThrowIfNull(serialized);

        try
        {
            string serializedString = Encoding.UTF8.GetString(serialized);

            var result = JsonSerializer.Deserialize(serializedString, StoreRecordSerializerContext.Default.StoreRecord)
                         ?? throw new StoreEntrySerializationException("Unable to deserialize Store Record.");
            return result;
        }
        catch (Exception exception)
        {
            throw new StoreEntrySerializationException("Unable to deserialize Store Record.", exception);
        }
    }
}
