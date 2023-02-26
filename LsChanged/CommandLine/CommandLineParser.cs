using LsChanged.Exceptions;

namespace LsChanged.CommandLine;

internal class CommandLineParser
{
    private readonly IReadOnlyDictionary<string, OptionRule> _rules;
    private readonly Action<CommandLineOptions> _emptyCommandLine;

    public CommandLineParser(IEnumerable<OptionRule> rules, Action<CommandLineOptions> emptyCommandLine)
    {
        ArgumentNullException.ThrowIfNull(rules);
        _rules = rules.ToDictionary(x => x.Prefix, x => x).AsReadOnly();

        ArgumentNullException.ThrowIfNull(emptyCommandLine);
        _emptyCommandLine = emptyCommandLine;
    }

    public CommandLineOptions Parse(string[] args)
    {
        var options = new CommandLineOptions();

        if(args.Length == 0)
        {
            _emptyCommandLine(options);
            return options;
        }

        int index = 0;
        while (index < args.Length)
        {
            string currentArg = args[index];
            
            var rule = _rules.GetValueOrDefault(currentArg)
                       ?? throw new CommandLineParseException($"Unknown command or option: '{currentArg}'.");

            if (index + 1 + rule.NumArguments > args.Length)
            {
                throw new CommandLineParseException(
                    $"Command or option '{rule.Prefix}' requires {rule.NumArguments} argument(s).");
            }

            var arguments = args.Skip(index + 1).Take(rule.NumArguments);
            try
            {
                rule.SetOption(options, arguments);
            }
            catch (Exception exception)
            {
                throw new CommandLineParseException(
                    $"Could not parse command/option '{rule.Prefix}' argument(s): {exception.Message}",
                    exception);
            }
            
            index += 1 + rule.NumArguments;
        }

        return options;
    }
}
