using System;
using System.IO;
using System.Linq;

namespace CleanProject.Service.Models
{
    public class SolutionInfo
    {
        private readonly bool useTempDirectory;
        private readonly string tempPath;

        public string Directory { get; private set; }

        public string Name { get; private set; }

        public string WorkingPath
        {
            get
            {
                return useTempDirectory ? tempPath : Directory;
            }
        }

        public SolutionInfo(string directory, bool useTempDirectory)
        {
            this.Directory = directory;
            this.useTempDirectory = useTempDirectory;

            this.Name = this.Directory.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Last();

            this.tempPath = Path.Combine(Path.GetTempPath(), this.Name);
        }
    }
}
