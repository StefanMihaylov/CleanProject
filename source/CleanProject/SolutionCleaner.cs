using System.Collections.Generic;

namespace CleanProject
{
    /// <summary>
    ///     Cleans solutions
    /// </summary>
    internal static class SolutionCleaner
    {
        internal static void CleanDirectories(this IEnumerable<SolutionInfo> directories)
        {
            foreach (var solutionInfo in directories)
            {
                CleanDirectory(solutionInfo.WorkingPath);
            }
        }

        internal static void CleanDirectory(this string directory)
        {
            // Remove standard directories
            directory.RemoveSubDirectories("bin", "obj", "TestResults", "_ReSharper*");

            // Remove directories provided on command line also
            directory.RemoveSubDirectories(Program.Options.RemoveDirectories);

            directory.DeleteFiles("*.ReSharper*", "*.suo");
            directory.DeleteFiles(Program.Options.RemoveFiles);

            if (Program.Options.RemoveSourceControl)
            {
                RemoveSourceControlBindings.Clean(directory);
            }
        }
    }
}