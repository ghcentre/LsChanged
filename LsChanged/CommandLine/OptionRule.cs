namespace LsChanged.CommandLine;

internal sealed record OptionRule
{
    public OptionRule(string prefix,
                      int numArguments,
                      Action<CommandLineOptions, IEnumerable<string>> setOption)
    {
        Prefix = prefix;
        NumArguments = Math.Max(numArguments, 0);
        SetOption = setOption;
    }

    public string Prefix { get; }

    public int NumArguments { get; }

    public Action<CommandLineOptions, IEnumerable<string>> SetOption { get; }
}
