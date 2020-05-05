using System.Collections.Generic;
using System.Linq;
using CleanProject.Service.Interfaces;

namespace CleanProject.Service.Models
{
    public class Options : IOptions
    {
        public IEnumerable<string> RemoveDirectories { get; private set; }

        public IEnumerable<string> RemoveFiles { get; private set; }

        public IEnumerable<string> RemoreSourceControl { get; private set; }

        public Options(IEnumerable<string> removeDirectories, IEnumerable<string> removeFiles,
            IEnumerable<string> remoreSourceControl)
        {
            var directories = new List<string>() { "bin", "obj", "TestResults", "_ReSharper*" };
            var files = new List<string>() { "*.ReSharper*", "*.suo" };
            var sourceControl = new List<string>() { "*.vssscc", "*.vspscc", "*.scc" };

            if (removeDirectories?.Any() == true)
            {
                directories.AddRange(removeDirectories);
            }

            if (removeFiles?.Any() == true)
            {
                files.AddRange(removeFiles);
            }

            if (remoreSourceControl?.Any() == true)
            {
                sourceControl.AddRange(remoreSourceControl);
            }

            this.RemoveDirectories = directories;
            this.RemoveFiles = files;
            this.RemoreSourceControl = sourceControl;
        }
    }
}
