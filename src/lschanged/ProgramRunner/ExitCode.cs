namespace LsChanged.ProgramRunner;

internal static class ExitCode
{
    public const int Success = 0;
    public const int NoFilesListed = 1;
    public const int SnapshotNotFound = 2;
    public const int IgnoreFileAlreadyExists = 3;
    public const int IgnoreFileNotFound = 4;
    public const int StoreUnaccessible = 5;
    public const int InvalidIgnoreFile = 6;
    public const int InvalidCommandLine = 253;
    public const int HelpDisplayed = 254;
    public const int GenericError = 255;
}