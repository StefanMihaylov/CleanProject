using System;
using System.Collections.Generic;
using System.IO;

namespace CleanProject
{
    internal static class DirectoryHelper
    {
        public static void RemoveSubDirectories(this string directory, string searchPattern)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            var directories = Directory.GetDirectories(directory, searchPattern, SearchOption.AllDirectories);
            foreach (var d in directories)
            {
                Delete(d);
            }
        }

        public static void RemoveSubDirectories(this string directory, params string[] searchPatterns)
        {
            foreach (var pattern in searchPatterns)
            {
                directory.RemoveSubDirectories(pattern);
            }
        }

        public static void RemoveSubDirectories(this string directory, IEnumerable<string> searchPatterns)
        {
            foreach (var pattern in searchPatterns)
            {
                directory.RemoveSubDirectories(pattern);
            }
        }

        internal static void CopyDirectory(this string source, string dest, bool subdirs, bool removeIfExists)
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
                Delete(dest);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }

            // Get the file contents of the directory to copy.
            var files = dir.GetFiles();

            foreach (var file in files)
            {
                // Create the path to the new copy of the file.
                var temppath = Path.Combine(dest, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If subdirs is true, copy the subdirectories.
            if (subdirs)
            {
                foreach (var subdir in dirs)
                {
                    // Create the subdirectory.
                    var temppath = Path.Combine(dest, subdir.Name);

                    // Copy the subdirectories.
                    CopyDirectory(subdir.FullName, temppath, true, removeIfExists);
                }
            }
        }

        internal static void Delete(this string directory)
        {
            try
            {
                if (Directory.Exists(directory))
                {
                    Program.WriteVerboseMessage("Removing {0}", directory);
                    directory.DeleteFiles();
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