namespace LsChanged.Settings;

internal sealed class FileInfoCollectorSettings
{
    public FileInfoCollectorSettings(FollowSymlinksMode followSymlinks)
    {
        FollowSymlinks = followSymlinks;
    }


    public FollowSymlinksMode FollowSymlinks { get; }
}
