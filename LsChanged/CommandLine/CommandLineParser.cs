using LsChanged.Exceptions;

namespace LsChanged.CommandLine;

internal class CommandLineParser(IEnumerable<OptionRule> rules, Action<CommandLineOptions> emptyCommandLine)
{
    private readonly IReadOnlyDictionary<string, OptionRule> _rules = rules.ToDictionary(x => x.Prefix, x => x).AsReadOnly();

    public CommandLineOptions Parse(string[] args)
    {
        var options = new CommandLineOptions();

        if(args.Length == 0)
        {
            emptyCommandLine(options);
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
