namespace LsChanged.Settings;

internal class FileInfoCollectorSettings
{
    public FileInfoCollectorSettings(FollowSymlinkSettings followSymlinkSettings)
    {
        FollowSymlinkSettings = followSymlinkSettings;
    }


    public FollowSymlinkSettings FollowSymlinkSettings { get; }
}
