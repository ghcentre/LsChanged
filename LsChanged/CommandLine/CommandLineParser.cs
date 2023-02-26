using LsChanged.Exceptions;

namespace LsChanged.CommandLine;

internal class CommandLineParser
{
    private readonly IReadOnlyDictionary<string, OptionRule> _rules;

    private readonly HashSet<OptionRule> _unsatisfiedRules;

    public CommandLineParser(IEnumerable<OptionRule> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);
        _rules = rules.ToDictionary(x => x.Prefix, x => x).AsReadOnly();

        _unsatisfiedRules = new HashSet<OptionRule>(rules.Where(x => x.Required));
    }

    public CommandLineOptions Parse(string[] args)
    {
        var result = new CommandLineOptions();

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
                rule.SetOption(result, arguments);
            }
            catch (Exception exception)
            {
                throw new CommandLineParseException(
                    $"Could not parse command/option '{rule.Prefix}' argument(s): {exception.Message}",
                    exception);
            }
            
            _unsatisfiedRules.Remove(rule);

            index += 1 + rule.NumArguments;
        }

        if (_unsatisfiedRules.Any())
        {
            var rule = _unsatisfiedRules.First();
            throw new CommandLineParseException($"Required command or option missing: '{rule.Prefix}'.");
        }

        return result;
    }
}
