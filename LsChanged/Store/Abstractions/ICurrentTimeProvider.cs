namespace LsChanged.Store.Abstractions;

internal interface ICurrentTimeProvider
{
    DateTime CurrentTime { get; }
}
