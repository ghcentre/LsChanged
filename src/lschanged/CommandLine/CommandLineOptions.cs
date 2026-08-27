using LsChanged.Collector;
using LsChanged.Compare;
using LsChanged.Exceptions;

namespace LsChanged.CommandLine;

internal sealed class CommandLineOptions
{
    public bool Verbose { get; set; } = false;

    public string? StorePath { get; set; } = default;

    public Command? Command { get; private set; } = default;

    public void SetCommand(Command command)
    {
        if (Command.HasValue)
        {
            throw new CommandLineParseException(
                $"The command could not be specified twice. Command: '{command.ToString().ToLowerInvariant()}'.");
        }
        Command = command;
    }

    #region Scan
    
    public string? ScanPath { get; set; } = default;

    public FollowSymlinksMode FollowSymlinksMode { get; set; } = FollowSymlinksMode.Skip;

    #endregion

    #region Compare

    public string? CompareOutputFile { get; set; } = default;

    public CompareMode CompareMode { get; set; } = CompareMode.LastPrevious;

    public int? NewCompareSnapshot { get; set; } = default;

    public int? OldCompareSnapshot { get; set; } = default;

    public CompareFileStates CompareFileStates { get; set; } = CompareFileStates.Added | CompareFileStates.Modified;

    public string? CompareRelativePath { get; set; } = default;

    #endregion

    #region Delete

    public int? SnaphotToDelete { get; set; } = default;

    #endregion

    #region NewIgnore/Compare

    public string? IgnoreFilePath { get; set; } = default;

    #endregion

    public void Validate()
    {
        switch (Command)
        {
            case CommandLine.Command.Help:
                return; // do not validate store path

            case CommandLine.Command.Scan:
                ValidateScanPath();
                ValidateFollowSymlinksMode();
                break;

            case CommandLine.Command.Compare:
                ValidateCompareModeSnapshots();
                break;

            case CommandLine.Command.List:
                break;

            case CommandLine.Command.Delete:
                break;

            case CommandLine.Command.Clear:
                break;

            case CommandLine.Command.NewIgnore:
                return; // do not validate store path

            default:
                throw new CommandLineParseException("No command specified.");
        }

        ValidateStorePath();
    }

    #region Scan

    private void ValidateScanPath()
    {
        if (string.IsNullOrWhiteSpace(ScanPath))
        {
            throw new CommandLineParseException("Scan path could not be empty.");
        }
    }

    private void ValidateFollowSymlinksMode()
    {
        bool defined = Enum.IsDefined(FollowSymlinksMode);
        if (!defined)
        {
            throw new CommandLineParseException("Invalid follow symlinks mode.");
        }
    }

    #endregion

    #region Compare

    private void ValidateCompareModeSnapshots()
    {
        if (CompareMode == CompareMode.SpecifiedSnapshots &&
            (!NewCompareSnapshot.HasValue || !OldCompareSnapshot.HasValue))
        {
            throw new CommandLineParseException(
                "Comparing between two specified snapshots requires exactly two snapshot ordinals.");
        }
    }

    #endregion

    private void ValidateStorePath()
    {
        if (string.IsNullOrWhiteSpace(StorePath))
        {
            throw new CommandLineParseException("Store path could not be empty.");
        }
    }
}
