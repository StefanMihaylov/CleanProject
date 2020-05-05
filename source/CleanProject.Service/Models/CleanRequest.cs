using System.Collections.Generic;
using CleanProject.Service.Interfaces;

namespace CleanProject.Service.Models
{
    public class CleanRequest
    {
        /// <summary>
        /// Directory to clean
        /// </summary>
        public IEnumerable<string> Directories { get; private set; }

        /// <summary>
        /// Copy clean and zip the project
        /// </summary>
        public bool ZipProject { get; private set; }

        /// <summary>
        /// Removes source control bindings
        /// </summary>
        public bool RemoveSourceControl { get; private set; }

        public bool ShowAllMessages { get; private set; }

        /// <summary>
        /// Zip file directory
        /// </summary>
        public string ZipDirectory { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IOptions AdditionalOptions { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool QuietMode { get; private set; }

        public CleanRequest(IEnumerable<string> directories, bool zipProject, bool removeSourceControl, bool showAllMessages,
            string zipDirectory, IOptions additionalOptions, bool quietMode)
        {
            this.Directories = directories;
            this.ZipProject = zipProject;
            this.RemoveSourceControl = removeSourceControl;
            this.ShowAllMessages = showAllMessages;
            this.ZipDirectory = zipDirectory;
            this.AdditionalOptions = additionalOptions;
            this.QuietMode = quietMode;
        }
    }
}
