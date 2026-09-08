namespace CopyFiles;

internal static class HelpPrinter
{
    public static void PrintHelp()
    {
        Console.WriteLine("""
            Usage: copyfiles source-directory destination-directory list-file

            source-directory: The directory containing the files to copy.
                              Must be an absolute path.
                              The directory must exist and be readable.

            destination-directory: The directory where the files will be copied to.
                                   Must be an absolute path.
                                   The directory must exist and be writable.

            list-file: A UTF-8 text file containing a list of files to copy.
                       Each line in the file should contain a relative path to a file
                       within the source directory. The file must exist and be readable.
            """);
    }
}
