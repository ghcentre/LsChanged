namespace LsChanged.ProgramRunner;

internal static class ExitCode
{
    public const int Success = 0;
    public const int InvalidCommandLine = 1;
    public const int SnapshotNotFound = 4;
    public const int HelpDisplayed = 254;
    public const int GenericError = 255;
}