using LsChanged.Store.Abstractions;

namespace LsChanged.Store;

internal sealed class CurrentTimeProvider : ICurrentTimeProvider
{
    public DateTime CurrentTime => DateTime.UtcNow;
}
