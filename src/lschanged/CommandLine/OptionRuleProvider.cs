using LsChanged.Collector;
using LsChanged.Compare;
using LsChanged.Exceptions;

namespace LsChanged.CommandLine;

internal static class OptionRuleProvider
{
    private static readonly OptionRule[] _rules =
    [
        #region Scan

        new("scan", 1,
            (o, a) =>
            {
                o.SetCommand(Command.Scan);
                o.ScanPath = a.First();
            }),

        new("-fs", 1,
            (o, a) =>
            {
                o.FollowSymlinksMode = Enum.Parse<FollowSymlinksMode>(a.First());
            }),

        #endregion

        #region Compare

        new("compare", 3,
            (o, a) =>
            {
                o.SetCommand(Command.Compare);

                string compareMode = a.First();
                SetCompareMode(o, compareMode);

                string compareStates = a.Skip(1).First();
                SetCompareStates(o, compareStates);

                o.CompareOutputFile = a.Skip(2).First();
            }),

        new("-rp", 1,
            (o, a) =>
            {
                o.CompareRelativePath = a.First();
            }),

        new("-i", 1,
            (o, a) =>
            {
                o.IgnoreFilePath = a.First();
            }),

	    #endregion

        #region List

		new("list", 0,
            (o, _) =>
            {
                o.SetCommand(Command.List);
            }),

        #endregion

        #region Delete

        new("delete", 1,
            (o, a) =>
            {
                o.SetCommand(Command.Delete);

                string snapshotOrdinal = a.First();

                if (snapshotOrdinal == "last")
                {
                    o.SnaphotToDelete = null;
                    return;
                }

                if (int.TryParse(snapshotOrdinal, out int snapshotInt))
                {
                    o.SnaphotToDelete = snapshotInt;
                    return;
                }

                throw new CommandLineParseException("Snapshot to delete must be 'last' or an integer value.");
            }),

	    #endregion

        #region Clear

        new("clear", 0,
            (o, _) =>
            {
                o.SetCommand(Command.Clear);
            }),

	    #endregion

        #region NewIgnore

        new("newignore", 1,
            (o, a) =>
            {
                o.SetCommand(Command.NewIgnore);
                o.IgnoreFilePath = a.First();
            }),

        #endregion


        new("-s", 1,
            (o, a) =>
            {
                o.StorePath = a.First();
            }),

        new("-v", 0,
            (o, _) =>
            {
                o.Verbose = true;
            }),
    ];

    #region Compare mode Argument Helpers

    private static void SetCompareMode(CommandLineOptions o, string modeString)
    {
        o.NewCompareSnapshot = o.OldCompareSnapshot = null;

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
                o.CompareMode = CompareMode.SpecifiedSnapshots;

                o.NewCompareSnapshot = int.Parse(snapshotOrdinalStrings[0]);
                o.OldCompareSnapshot = int.Parse(snapshotOrdinalStrings[1]);

                return;
            }
        }
        throw new CommandLineParseException(
            "Compare mode argument must be lp, lf, or exactly two numbers separated by comma.");
    }

    private static void SetCompareStates(CommandLineOptions o, string compareStates)
    {
        var fileStates = CompareFileStates.None;

        var states = compareStates.Contains(',')
                        ? compareStates.Split(",")
                        : compareStates.ToCharArray().Select(x => new string(x, 1));

        foreach (string stateString in states)
        {
            var state = stateString switch
            {
                "a" => CompareFileStates.Added,
                "m" => CompareFileStates.Modified,
                "u" => CompareFileStates.Unmodified,
                "d" => CompareFileStates.Deleted,
                _ => throw new CommandLineParseException(
                        "Compare file states requires any combination of a,m,u,d in any order.")
            };
            fileStates |= state;
        }
        o.CompareFileStates = fileStates;
    }

    #endregion

    public static IEnumerable<OptionRule> Rules => _rules;
}
