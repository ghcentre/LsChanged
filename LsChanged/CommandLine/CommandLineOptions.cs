using LsChanged.Exceptions;
using LsChanged.Settings;

namespace LsChanged.CommandLine;

internal class CommandLineOptions
{
    public bool Verbose { get; set; } = false;

    public string? StorePath { get; set; } = default;

    public Command? Command { get; private set; } = default;

    public void SetCommand(Command command)
    {
        if (Command.HasValue)
        {
            throw new CommandLineParseException(
                $"The command could not be specИified twice. Command: '{command.ToString().ToLowerInvariant()}'.");
        }
        Command = command;
    }

    public string? ScanPath { get; set; } = default;

    public FollowSymlinksMode FollowSymlinks { get; set; } = FollowSymlinksMode.Skip;

    public void Validate()
    {
        ValidateStorePath();

        switch (Command)
        {
            case CommandLine.Command.Scan:
                ValidateScanPath();
                break;

            case CommandLine.Command.Compare:
                break;

            case CommandLine.Command.List:
                break;

            case CommandLine.Command.Delete:
                break;

            case CommandLine.Command.Clear:
                break;

            default:
                throw new CommandLineParseException("No command specified.");
        }
    }

    private void ValidateScanPath()
    {
        if (string.IsNullOrWhiteSpace(ScanPath))
        {
            throw new CommandLineParseException("Scan path could not be empty.");
        }
    }

    private void ValidateStorePath()
    {
        if (string.IsNullOrWhiteSpace(StorePath))
        {
            throw new CommandLineParseException("Store path could not be empty.");
        }
    }
}
