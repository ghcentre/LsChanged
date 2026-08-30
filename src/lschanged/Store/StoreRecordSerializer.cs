using LsChanged.Exceptions;
using LsChanged.Store.Abstractions;
using LsChanged.Store.Serialization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace LsChanged.Store;

internal sealed class StoreRecordSerializer : IStoreRecordSerializer
{
    private static readonly JsonSerializerOptions _serializerOptions =
        new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };

    public byte[] Serialize(IStoreRecord storeRecord)
    {
        ArgumentNullException.ThrowIfNull(storeRecord);

        if (storeRecord is not StoreRecord recordImpl)
        {
            throw new StoreEntrySerializationException("Unsupported Store Record implementation.");
        }

        var context = new StoreRecordSerializerContext(_serializerOptions);
        string resultString = JsonSerializer.Serialize(recordImpl, context.StoreRecord);

        byte[] result = Encoding.UTF8.GetBytes(resultString);
        return result;
    }
}
