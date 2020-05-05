using System;
using System.Collections.Generic;
using System.IO;

namespace CleanProject
{
    internal static class FileHelper
    {
        internal static void DeleteFiles(this string directory, params string[] searchPatterns)
        {
            foreach (var searchPattern in searchPatterns)
            {
                DeleteFiles(directory, searchPattern);
            }
        }

        internal static void DeleteFiles(this string directory, string searchPattern)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            var files = Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    Program.WriteVerboseMessage("Deleting file {0}", file);
                    TurnOffReadOnlyFlag(file);
                    File.Delete(file);
                }
                catch (IOException ex)
                {
                    throw new ApplicationException($"Error removing file {file} - {ex.Message}", ex);
                }
            }
        }

        internal static void DeleteFiles(this string directory)
        {
            DeleteFiles(directory, "*");
        }

        internal static void DeleteFiles(this string directory, IEnumerable<string> searchPatterns)
        {
            foreach (var searchPattern in searchPatterns)
            {
                DeleteFiles(directory, searchPattern);
            }
        }

        /// <summary>
        ///     Turns off the read only flag on a file.
        /// </summary>
        /// <param name="file">
        ///     The file to change.
        /// </param>
        /// <returns>
        ///     Returns true if the read only flag was set.
        /// </returns>
        internal static bool TurnOffReadOnlyFlag(this string file)
        {
            var retValue = false;
            var attribs = File.GetAttributes(file);
            if ((attribs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                retValue = true;
                File.SetAttributes(file, attribs & ~FileAttributes.ReadOnly);
            }
            return (retValue);
        }

        /// <summary>
        ///     Turns on the read only flag for a file.
        /// </summary>
        /// <param name="file">
        ///     The file to change.
        /// </param>
        internal static void TurnOnReadOnlyFlag(this string file)
        {
            var attribs = File.GetAttributes(file);
            File.SetAttributes(file, attribs | FileAttributes.ReadOnly);
        }
    }
}