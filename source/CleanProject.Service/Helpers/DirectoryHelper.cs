using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanProject.Service.Interfaces;

namespace CleanProject.Service.Helpers
{
    public class DirectoryHelper : IDirectoryHelper
    {
        private readonly INotificationHelper _notificationHelper;
        private readonly IFileHelper _fileHelper;

        public DirectoryHelper(INotificationHelper notificationHelper, IFileHelper fileHelper)
        {
            this._notificationHelper = notificationHelper;
            this._fileHelper = fileHelper;
        }

        public void RemoveSubDirectories(string directory, IEnumerable<string> searchPatterns, bool skipNotifications)
        {
            if (searchPatterns?.Any() != true)
            {
                return;
            }

            foreach (var pattern in searchPatterns)
            {
                this.RemoveSubDirectories(directory, pattern, skipNotifications);
            }
        }

        public void CopyDirectory(string source, string destination, bool subdirs, bool removeIfExists)
        {
            var dir = new DirectoryInfo(source);
            var dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + source);
            }

            // Removes the directory if it already exists
            if (removeIfExists)
            {
                this.DeleteDirectory(destination, true);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            // Get the file contents of the directory to copy.
            var files = dir.GetFiles();

            foreach (var file in files)
            {
                // Create the path to the new copy of the file.
                var temppath = Path.Combine(destination, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If subdirs is true, copy the subdirectories.
            if (subdirs)
            {
                foreach (var subdir in dirs)
                {
                    // Create the subdirectory.
                    var temppath = Path.Combine(destination, subdir.Name);

                    // Copy the subdirectories.
                    CopyDirectory(subdir.FullName, temppath, true, removeIfExists);
                }
            }
        }

        private void RemoveSubDirectories(string directory, string searchPattern, bool skipNotifications)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            this._notificationHelper.WriteColorMessage(ConsoleColor.Green, $" --- Directory SearchPattern: '{searchPattern}' ---");

            var directories = Directory.GetDirectories(directory, searchPattern, SearchOption.AllDirectories);
            foreach (var d in directories)
            {
                DeleteDirectory(d, skipNotifications);
            }
        }

        public void DeleteDirectory(string directory, bool skipNotifications)
        {
            try
            {
                if (Directory.Exists(directory))
                {
                    this._notificationHelper.WriteVerboseMessage($"Removing {directory}");

                    this._fileHelper.DeleteFiles(directory, new[] { "*" }, skipNotifications);
                    var retry = 0;

                    // Sometimes you encounter a directory is not empty error immediately after deleting all the files.
                    // This loop will retry 3 times
                    while (retry < 3)
                    {
                        try
                        {
                            Directory.Delete(directory, true);
                            break;
                        }
                        catch (IOException)
                        {
                            retry++;
                            if (retry > 3)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (IOException ioException)
            {
                throw new ApplicationException($"Error removing directory {directory}: {ioException.Message}");
            }
        }
    }
}
