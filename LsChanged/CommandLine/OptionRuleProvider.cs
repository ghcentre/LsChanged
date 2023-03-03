using LsChanged.Collector;
using LsChanged.Compare;
using LsChanged.Exceptions;

namespace LsChanged.CommandLine;

internal class OptionRuleProvider
{
    public static IEnumerable<OptionRule> Rules
    {
        get
        {
            var rules = new[]
            {
                #region Scan

                new OptionRule("scan", 1,
                    (o, a) =>
                    {
                        o.SetCommand(Command.Scan);
                        o.ScanPath = a.First();
                    }),

                new OptionRule("-fs", 1,
                    (o, a) =>
                    {
                        o.FollowSymlinksMode = Enum.Parse<FollowSymlinksMode>(a.First());
                    }),

                #endregion

                #region Compare

                new OptionRule("compare", 1,
                    (o, a) =>
                    {
                        o.SetCommand(Command.Compare);
                        o.CompareOutputFile = a.First();
                    }),

                new OptionRule("-cm", 1,
                    (o, a) =>
                    {
                        string modeString = a.First();
                        if (modeString == "lp")
                        {
                            o.CompareMode = CompareMode.LastPrevious;
                            return;
                        }
                        if (modeString == "lf")
                        {
                            o.CompareMode = CompareMode.LastFirst;
                            return;
                        }

                        if (modeString.Count(c => c == ',') == 1)
                        {
                            string[] snapshotOrdinalStrings = modeString
                                .Split(',')
                                .Where(x => x.All(c => char.IsAsciiDigit(c)))
                                .ToArray();
                            if (snapshotOrdinalStrings.Length == 2)
                            {
                                o.NewCompareSnapshot = int.Parse(snapshotOrdinalStrings[0]);
                                o.OldCompareSnapshot = int.Parse(snapshotOrdinalStrings[1]);

                                return;
                            }
                        }
                        throw new CommandLineParseException(
                            "Compare mode option must be lp, lf, or exactly two numeric arguments separated by comma.");
                    }),

	            #endregion


                new OptionRule("list", 0,
                    (o, _) =>
                    {
                        throw new NotImplementedException();
                    }),

                new OptionRule("delete", 1,
                    (o, a) =>
                    {
                        throw new NotImplementedException();
                    }),

                new OptionRule("clear", 0,
                    (o, _) =>
                    {
                        throw new NotImplementedException();
                    }),

               new OptionRule("-s", 1,
                    (o, a) =>
                    {
                        o.StorePath = a.First();
                    }),

                new OptionRule("-v", 0,
                    (o, _) =>
                    {
                        o.Verbose = true;
                    }),
            };

            return rules;
        }
    }
}
