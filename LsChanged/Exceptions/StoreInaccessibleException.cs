namespace LsChanged.Exceptions;

internal class StoreInaccessibleException : Exception
{
    public StoreInaccessibleException(Exception? innerException) : base(innerException?.Message, innerException)
    {
    }
}
