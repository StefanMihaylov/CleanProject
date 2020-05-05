using System;
using System.Collections.Generic;
using System.IO;
using CleanProject.Service.Interfaces;
using CleanProject.Service.Models;
using Ionic.Zip;

namespace CleanProject.Service.Processors
{
    public class ZipHelper : IZipHelper
    {
        private readonly INotificationHelper _notificationHelper;
        private readonly IDirectoryHelper _directoryHelper;

        public ZipHelper(INotificationHelper notificationHelper, IDirectoryHelper directoryHelper)
        {
            this._notificationHelper = notificationHelper;
            this._directoryHelper = directoryHelper;
        }

        public void ZipDirectories(IEnumerable<SolutionInfo> directoryInfos, string zipDirectory)
        {
            foreach (var directoryInfo in directoryInfos)
            {
                this.CreateZipFile(directoryInfo, zipDirectory);
            }
        }

        private void CreateZipFile(SolutionInfo directoryInfo, string zipDirectory)
        {
            using (var zip = new ZipFile())
            {
                zip.AddDirectory(directoryInfo.WorkingPath);

                // No ZipDirectory provided
                if (string.IsNullOrWhiteSpace(zipDirectory))
                {
                    // Default to the parent folder of the solution
                    zipDirectory = Path.GetFullPath(Path.Combine(directoryInfo.Directory, ".."));
                    if (!Directory.Exists(zipDirectory))
                    {
                        // No parent folder then use the solution directory
                        zipDirectory = directoryInfo.Directory;
                    }
                }

                var zipName = Path.Combine(zipDirectory, directoryInfo.Name + ".zip");

                zip.Save(zipName);
                this._notificationHelper.WriteColorMessage(ConsoleColor.Yellow, $"Created zip file {zipName}");

                this._directoryHelper.Delete(directoryInfo.WorkingPath);
            }
        }
    }
}
