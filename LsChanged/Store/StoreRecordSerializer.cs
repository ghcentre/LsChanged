using LsChanged.Exceptions;
using LsChanged.Store.Abstractions;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LsChanged.Store;

internal class StoreRecordSerializer : IStoreRecordSerializer
{
    public string Serialize(IStoreRecord storeRecord)
    {
        ArgumentNullException.ThrowIfNull(storeRecord);

        if (storeRecord is not StoreRecord recordImpl)
        {
            throw new StoreEntrySerializationException("Unsupported Store Record implementation.");
        }

        var options = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };
        string result = JsonSerializer.Serialize(recordImpl, options);

        return result;
    }
}
