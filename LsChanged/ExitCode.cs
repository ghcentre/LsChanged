namespace LsChanged;

internal static class ExitCode
{
    public const int Success = 0;
    public const int InvalidCommandLineArg = 1;
    public const int InvalidPathSpecified = 2;
    public const int WriteError = 3;
    public const int GenericError = 255;
}
