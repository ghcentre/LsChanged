namespace LsChanged.Store;

internal class CurrentTimeProvider : ICurrentTimeProvider
{
    public DateTime CurrentTime => DateTime.UtcNow;
}
