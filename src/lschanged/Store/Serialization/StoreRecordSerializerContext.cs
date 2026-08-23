using System.Text.Json.Serialization;

namespace LsChanged.Store.Serialization;

[JsonSerializable(typeof(StoreRecord))]
internal partial class StoreRecordSerializerContext : JsonSerializerContext
{
}
