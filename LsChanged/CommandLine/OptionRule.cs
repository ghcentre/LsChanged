namespace LsChanged.CommandLine;

internal sealed record OptionRule
{
    public OptionRule(string prefix,
                      int numArguments,
                      bool required,
                      Action<CommandLineOptions, IEnumerable<string>> setOption)
    {
        ArgumentException.ThrowIfNullOrEmpty(prefix);
        Prefix = prefix;

        Required = required;

        NumArguments = Math.Max(numArguments, 0);

        ArgumentNullException.ThrowIfNull(setOption);
        SetOption = setOption;
    }

    public string Prefix { get; }

    public int NumArguments { get; }

    public bool Required { get; }

    public Action<CommandLineOptions, IEnumerable<string>> SetOption { get; }
}
