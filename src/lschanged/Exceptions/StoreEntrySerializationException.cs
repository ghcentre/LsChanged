namespace LsChanged.Exceptions;

internal sealed class StoreEntrySerializationException : Exception
{
    public StoreEntrySerializationException(string? message) : base(message)
    {
    }

    public StoreEntrySerializationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
