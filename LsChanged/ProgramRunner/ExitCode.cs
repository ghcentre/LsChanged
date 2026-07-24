namespace LsChanged.ProgramRunner;

internal static class ExitCode
{
    public const int Success = 0;
    public const int NoFilesListed = 1;
    public const int SnapshotNotFound = 2;
    public const int FileAlreadyExists = 3;
    public const int InvalidCommandLine = 253;
    public const int HelpDisplayed = 254;
    public const int GenericError = 255;
}