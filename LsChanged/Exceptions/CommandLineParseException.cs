namespace LsChanged.Exceptions;

internal class CommandLineParseException : InvalidOperationException
{
    public CommandLineParseException(string message)
        : base(message)
    {
    }

    public CommandLineParseException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
