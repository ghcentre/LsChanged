namespace LsChanged.Environment;

internal static class ExitCode
{
    public const int Success = 0;
    public const int InvalidCommandLine = 1;
    public const int InvalidPathSpecified = 2;
    public const int WriteError = 3;
    public const int HelpDisplayed = 254;
    public const int GenericError = 255;
}
