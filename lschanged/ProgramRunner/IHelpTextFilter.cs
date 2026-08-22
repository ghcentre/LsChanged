namespace lschanged.ProgramRunner;

internal interface IHelpTextFilter
{
    public IEnumerable<string> Filter(string text);
}
