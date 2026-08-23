using LsChanged.CommandLine;
using LsChanged.Exceptions;
using LsChanged.Logging;
using System.Text.RegularExpressions;

namespace LsChanged.ProgramRunner;

internal sealed class IgnoreProcessor : IIgnoreProcessor
{
    private const string _optionsPrefix = "options=";
    private const string _regexPrefix = "regex=";
    private const string _caseInsensitiveOption = "+i";
    private const string _caseSensitiveOption = "-i";

    private record RuleRecord(int Index, Regex Regex);

    private readonly bool _isWindows = false;
    private readonly string? _ignoreFilePath;
    private readonly ILogger _logger;
    private readonly List<RuleRecord> _rules;

    public IgnoreProcessor(ILogger logger, CommandLineOptions commandLineOptions)
    {
        _isWindows = OperatingSystem.IsWindows();
        _ignoreFilePath = commandLineOptions.IgnoreFilePath;
        _logger = logger;
        _rules = LoadRules(commandLineOptions.IgnoreFilePath);
    }

    #region Load Rules

    private List<RuleRecord> LoadRules(string? ignoreFilePath)
    {
        if (string.IsNullOrEmpty(ignoreFilePath))
        {
            return [];
        }

        if (!File.Exists(ignoreFilePath))
        {
            throw new IgnoreFileNotFoundException(ignoreFilePath);
        }

        try
        {
            var rules = ProcessRules(ignoreFilePath);
            _logger.Debug("Loaded {0} ignore rules from {1}.", rules.Count, ignoreFilePath);
            return rules;
        }
        catch (Exception exception)
        {
            throw new InvalidIgnoreFileException(ignoreFilePath, exception);
        }
    }

    private List<RuleRecord> ProcessRules(string ignoreFilePath)
    {
        var lines = File.ReadAllLines(ignoreFilePath).Select((line, index) => (Line: line, Index: index + 1));
        var nonCommentLines = lines.Where(x => !string.IsNullOrWhiteSpace(x.Line) && !x.Line.StartsWith('#')).ToList();

        var result = new List<RuleRecord>(nonCommentLines.Count);

        foreach (var lineInfo in nonCommentLines)
        {
            var regexOptions = GetDefaultRegexOptions();
            var lineSpan = lineInfo.Line.AsSpan();
            int regexIndex = 0;

            if (lineSpan.StartsWith(_optionsPrefix, StringComparison.Ordinal))
            {
                var optionsItself = lineSpan[_optionsPrefix.Length..];
                int spaceIndex = optionsItself.IndexOf(' ');
                if (spaceIndex == -1)
                {
                    throw new ArgumentException(
                        $"No regex pattern provided after options; line {lineInfo.Index}, file {ignoreFilePath}.");
                }

                var optionsTrimmed = optionsItself[..spaceIndex];
                regexOptions = ProcessOptions(regexOptions, optionsTrimmed, lineInfo.Index, ignoreFilePath);

                regexIndex = lineSpan.IndexOf(' ') + 1;
            }
            
            var regexItself = lineSpan[regexIndex..];
            if (!regexItself.StartsWith(_regexPrefix, StringComparison.Ordinal))
            {
                throw new ArgumentException($"No regex pattern provided; line {lineInfo.Index}, file {ignoreFilePath}.");
            }

            int regexPrefixIndex = regexItself.IndexOf(_regexPrefix, StringComparison.Ordinal);
            var regexPattern = regexItself[(regexPrefixIndex + _regexPrefix.Length)..];

            var regex = new Regex(regexPattern.ToString(), regexOptions);
            result.Add(new RuleRecord(lineInfo.Index, regex));
        }

        return result;
    }

    private static RegexOptions ProcessOptions(RegexOptions options, ReadOnlySpan<char> optionsSpan, int index, string filePath)
    {
        if (optionsSpan.Equals(_caseInsensitiveOption.AsSpan(), StringComparison.Ordinal))
        {
            return options | RegexOptions.IgnoreCase;
        }

        if (optionsSpan.Equals(_caseSensitiveOption.AsSpan(), StringComparison.Ordinal))
        {
            return options & ~RegexOptions.IgnoreCase;
        }

        throw new InvalidOperationException($"Unknown option '{optionsSpan}'; line {index}, file {filePath}.");
    }

    private RegexOptions GetDefaultRegexOptions()
    {
        var result = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        if (_isWindows)
        {
            result |= RegexOptions.IgnoreCase;
        }

        return result;
    }

    #endregion

    #region Process Ignores

    public IEnumerable<string> FilterIgnored(IEnumerable<string> filePaths)
    {
        return _rules.Count == 0 ? filePaths : FilterIgnoredInternal(filePaths);
    }

    private IEnumerable<string> FilterIgnoredInternal(IEnumerable<string> filePaths)
    {
        foreach (string? filePath in filePaths)
        {
            string? normalizedFilePath = _isWindows ? filePath.Replace('\\', '/') : filePath;

            var matchingRule = _rules.FirstOrDefault(rule => rule.Regex.IsMatch(normalizedFilePath));
            if (matchingRule != null)
            {
                _logger.Debug(
                    "Ignored: '{0}': line #{1}, ignore file {2}.",
                    filePath,
                    matchingRule.Index,
                    _ignoreFilePath!);
            }
            else
            {
                _logger.Debug("Not ignored: '{0}'", filePath);
                yield return filePath;
            }
        }
    }

    #endregion
}
