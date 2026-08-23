namespace LsChanged.ProgramRunner;

internal interface IHelpTextFilter
{
    public IEnumerable<string> Filter(string text);
}
