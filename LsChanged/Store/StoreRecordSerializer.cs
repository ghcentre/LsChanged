using LsChanged.Exceptions;
using LsChanged.Store.Abstractions;
using LsChanged.Store.Serialization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LsChanged.Store;

internal class StoreRecordSerializer : IStoreRecordSerializer
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        TypeInfoResolver = StoreRecordSerializerContext.Default,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true
    };

    public string Serialize(IStoreRecord storeRecord)
    {
        ArgumentNullException.ThrowIfNull(storeRecord);

        if (storeRecord is not StoreRecord recordImpl)
        {
            throw new StoreEntrySerializationException("Unsupported Store Record implementation.");
        }

        string result = JsonSerializer.Serialize(recordImpl, typeof(StoreRecord), _serializerOptions);

        return result;
    }
}
