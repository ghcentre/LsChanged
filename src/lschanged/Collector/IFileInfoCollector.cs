namespace LsChanged.Collector;

internal interface IFileInfoCollector
{
    IReadOnlyDictionary<string, FileStatus> Collect(string path);
}