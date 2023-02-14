namespace LsChanged.Store;

internal interface ICurrentTimeProvider
{
    DateTime CurrentTime { get; }
}
