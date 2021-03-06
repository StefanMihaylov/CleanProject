﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanProject.Service.Interfaces;

namespace CleanProject.Service.Helpers
{
    public class FileHelper : IFileHelper
    {
        private readonly INotificationHelper _notificationHelper;

        public FileHelper(INotificationHelper notificationHelper)
        {
            this._notificationHelper = notificationHelper;
        }

        public void DeleteFiles(string directory, IEnumerable<string> searchPatterns, bool skipNotifications)
        {
            if (searchPatterns?.Any() != true)
            {
                return;
            }

            foreach (var searchPattern in searchPatterns)
            {
                this.DeleteFiles(directory, searchPattern, skipNotifications);
            }
        }

        private void DeleteFiles(string directory, string searchPattern, bool skipNotifications)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            if (!skipNotifications)
            {
                this._notificationHelper.WriteColorMessage(ConsoleColor.Green, $" --- File SearchPattern: '{searchPattern}' ---");
            }

            var files = Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    if (!skipNotifications)
                    {
                        this._notificationHelper.WriteVerboseMessage($"Deleting file {file}");
                    }

                    this.TurnOffReadOnlyFlag(file);
                    File.Delete(file);
                }
                catch (IOException ex)
                {
                    throw new ApplicationException($"Error removing file {file} - {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        ///     Turns on the read only flag for a file.
        /// </summary>
        /// <param name="file">
        ///     The file to change.
        /// </param>
        public void TurnOnReadOnlyFlag(string file)
        {
            var attribs = File.GetAttributes(file);
            File.SetAttributes(file, attribs | FileAttributes.ReadOnly);
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
        public bool TurnOffReadOnlyFlag(string file)
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
    }
}
