namespace CopyFiles;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length != 3)
        {
            HelpPrinter.PrintHelp();
            return ExitCodes.HelpDisplayed;
        }

        try
        {
            var filePaths = ListFileReader.GetFilePaths(args[2]);
            FileCopier.CopyFiles(args[0], args[1], filePaths);
            return ExitCodes.Success;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Error: {exception.Message}");
            return ExitCodes.UnhandledError;
        }
    }
}
