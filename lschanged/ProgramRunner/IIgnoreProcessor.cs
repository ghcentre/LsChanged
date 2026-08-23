namespace LsChanged.ProgramRunner;

internal interface IIgnoreProcessor
{
    public IEnumerable<string> FilterIgnored(IEnumerable<string> filePaths);
}
