using LsChanged.Settings;

namespace LsChanged.CommandLine;

internal class OptionRuleProvider
{
    public static IEnumerable<OptionRule> Rules
    {
        get
        {
            var rules = new[]
            {
                new OptionRule("-s", 1, true,
                    (o, a) =>
                    {
                        o.StorePath = a.First();
                    }),

                new OptionRule("-v", 0, false,
                    (o, _) =>
                    {
                        o.Verbose = true;
                    }),

                new OptionRule("scan", 1, false,
                    (o, a) =>
                    {
                        o.SetCommand(Command.Scan);
                        o.ScanPath = a.First();
                    }),

                new OptionRule("-fs", 1, false,
                    (o, a) =>
                    {
                        o.FollowSymlinks = Enum.Parse<FollowSymlinksMode>(a.First());
                    }),

                new OptionRule("compare", 1, false,
                    (o, a) =>
                    {
                        throw new NotImplementedException();
                    }),

                new OptionRule("list", 0, false,
                    (o, _) =>
                    {
                        throw new NotImplementedException();
                    }),

                new OptionRule("delete", 1, false,
                    (o, a) =>
                    {
                        throw new NotImplementedException();
                    }),

                new OptionRule("clear", 0, false,
                    (o, _) =>
                    {
                        throw new NotImplementedException();
                    }),
            };

            return rules;
        }
    }
}
