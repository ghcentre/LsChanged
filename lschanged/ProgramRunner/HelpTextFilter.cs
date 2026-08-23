namespace LsChanged.ProgramRunner;

internal sealed class HelpTextFilter : IHelpTextFilter
{
    public IEnumerable<string> Filter(string text)
    {
        var lines = text.Replace("\r", string.Empty).Split('\n');

        var filters = CreateFilterStack();

        foreach (var line in lines)
        {
            bool isPreprocessDirective = TryPreprocess(line, filters);
            if (!isPreprocessDirective)
            {
                var currentFilter = filters.Peek();
                if (currentFilter())
                {
                    yield return line;
                }
            }
        }
    }

    private static Stack<Func<bool>> CreateFilterStack()
    {
        var filters = new Stack<Func<bool>>();

        static bool defaultFilter() => true;
        filters.Push(defaultFilter);

        return filters;
    }

    private static bool TryPreprocess(string line, Stack<Func<bool>> filters)
    {
        static bool isWindows() => OperatingSystem.IsWindows();
        static bool isNotWindows() => !OperatingSystem.IsWindows();

        if (line == "#if WINDOWS")
        {
            filters.Push(isWindows);
            return true;
        }
        if (line == "#if !WINDOWS")
        {
            filters.Push(isNotWindows);
            return true;
        }
        if (line == "#endif")
        {
            filters.Pop();
            return true;
        }
        return false;
    }
}
