using System;
using System.Collections.Generic;
using System.Text;

namespace CopyFiles;

internal static class ExitCodes
{
    public const int Success = 0;
    public const int DirectoryDoesNotExist = 1;
    public const int DirectoryNotReadable = 2;
    public const int DirectoryNotWritable = 3;
    public const int FileDoesNotExist = 4;
    public const int FileNotReadable = 5;
    public const int InvalidFilePath = 6;
    public const int IOError = 32;
    public const int HelpDisplayed = 253;
    public const int UnhandledError = 254;
}
