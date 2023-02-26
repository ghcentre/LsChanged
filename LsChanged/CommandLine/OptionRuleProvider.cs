using LsChanged.Collector;

namespace LsChanged.CommandLine;

internal class OptionRuleProvider
{
    public static IEnumerable<OptionRule> Rules
    {
        get
        {
            var rules = new[]
            {
                new OptionRule("scan", 1,
                    (o, a) =>
                    {
                        o.SetCommand(Command.Scan);
                        o.ScanPath = a.First();
                    }),

                new OptionRule("-fs", 1,
                    (o, a) =>
                    {
                        o.FollowSymlinks = Enum.Parse<FollowSymlinksMode>(a.First());
                    }),

                new OptionRule("compare", 1,
                    (o, a) =>
                    {
                        throw new NotImplementedException();
                    }),

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
