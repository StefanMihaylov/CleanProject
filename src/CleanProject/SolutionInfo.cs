using System;
using System.IO;
using System.Linq;

namespace CleanProject
{
    internal class SolutionInfo
    {
        private string GetTempPath()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = Directory.Split(new[] {Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries).Last();
            }

            tempPath = Path.Combine(Path.GetTempPath(), Name);

            return tempPath;
        }

        private string tempPath;

        public string WorkingPath => Program.Options.ZipProject ? TempPath : Directory;

        public string TempPath => string.IsNullOrWhiteSpace(tempPath) ? GetTempPath() : tempPath;

        internal string Directory { get; set; }

        internal string Name { get; private set; }
    }
}