namespace LsChanged.CommandLine;

internal sealed record OptionRule
{
    public OptionRule(string prefix,
                      int numArguments,
                      Action<CommandLineOptions, IEnumerable<string>> setOption)
    {
        ArgumentException.ThrowIfNullOrEmpty(prefix);
        Prefix = prefix;

        NumArguments = Math.Max(numArguments, 0);

        ArgumentNullException.ThrowIfNull(setOption);
        SetOption = setOption;
    }

    public string Prefix { get; }

    public int NumArguments { get; }

    public Action<CommandLineOptions, IEnumerable<string>> SetOption { get; }
}
