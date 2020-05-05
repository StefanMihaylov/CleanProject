using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CleanProject.Service.Interfaces;
using CleanProject.Service.Models;

namespace CleanProject.Service.Processors
{
    public class SolutionCleaner : ISolutionCleaner
    {
        private readonly IDirectoryHelper _directoryHelper;
        private readonly IFileHelper _fileHelper;
        private readonly IRemoveSourceControlBindings _removeSourceControlBindings;
        private readonly IOptions _options;
        private readonly INotificationHelper _notificationHelper;

        public SolutionCleaner(IDirectoryHelper directoryHelper, IFileHelper fileHelper, IOptions options,
            IRemoveSourceControlBindings removeSourceControlBindings, INotificationHelper notificationHelper)
        {
            this._directoryHelper = directoryHelper;
            this._fileHelper = fileHelper;
            this._removeSourceControlBindings = removeSourceControlBindings;
            this._options = options;
            this._notificationHelper = notificationHelper;
        }

        public void CleanDirectories(IEnumerable<SolutionInfo> directories, IOptions additionalOptions, bool removeSourceControl)
        {
            foreach (var solutionInfo in directories)
            {
                CleanDirectory(solutionInfo.WorkingPath, additionalOptions, removeSourceControl);
            }
        }

        public IEnumerable<SolutionInfo> GetDirectories(IEnumerable<string> directories, bool zipProject)
        {
            var result = directories.Select(directory => GetDirectories(directory, zipProject));

            foreach (var solutionInfo in result)
            {
                this._notificationHelper.WriteMessage($"Cleaning Solution Directory {solutionInfo.WorkingPath}");
            }

            return result;
        }

        private void CleanDirectory(string directory, IOptions additionalOptions, bool removeSourceControl)
        {
            this._directoryHelper.RemoveSubDirectories(directory, this._options.RemoveDirectories);
            this._directoryHelper.RemoveSubDirectories(directory, additionalOptions?.RemoveDirectories);

            this._fileHelper.DeleteFiles(directory, this._options.RemoveFiles);
            this._fileHelper.DeleteFiles(directory, additionalOptions?.RemoveFiles);

            if (removeSourceControl)
            {
                this._removeSourceControlBindings.Clean(directory, this._options.RemoreSourceControl);
                this._removeSourceControlBindings.Clean(directory, additionalOptions?.RemoreSourceControl);
            }
        }
        
        private SolutionInfo GetDirectories(string directory, bool zipProject)
        {
            var sb = new StringBuilder(255);
            GetLongPathName(directory, sb, sb.Capacity);

            string longDirectoryName = sb.ToString();

            if (zipProject)
            {
                if (!Directory.Exists(directory))
                {
                    throw new ApplicationException($"Directory \"{directory}\" does not exist");
                }

                this._notificationHelper.WriteMessage($"Copying solution {directory} to temporary directory");

                var solutionInfo = new SolutionInfo(longDirectoryName, true);

                this._directoryHelper.CopyDirectory(directory, solutionInfo.WorkingPath, true, true);

                return solutionInfo;
            }
            else
            {
                return new SolutionInfo(longDirectoryName, false);
            }
        }

        /// <summary>
        ///     Converts the specified path to its long form.
        ///     <para>
        ///         PInvode map to the
        ///         <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa364980(v=vs.85).aspx">
        ///             <strong xmlns="http://www.w3.org/1999/xhtml">GetLongPathName</strong>
        ///         </a>
        ///         function
        ///     </para>
        /// </summary>
        /// <param name="path">
        ///     The path to be converted.
        /// </param>
        /// <param name="pszPath">
        ///     A pointer to the buffer to receive the long path.
        /// </param>
        /// <param name="cchPath">
        ///     The size of the buffer lpszLongPath points to, in TCHARs.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is the length, in TCHARs, of the string copied to lpszLongPath, not
        ///     including the terminating null character.
        ///     <para>
        ///         If the lpBuffer buffer is too small to contain the path, the return value is the size, in TCHARs, of the
        ///         buffer that is required to hold the path and the terminating null character.
        ///     </para>
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetLongPathName(string path, StringBuilder pszPath, int cchPath);
    }
}
