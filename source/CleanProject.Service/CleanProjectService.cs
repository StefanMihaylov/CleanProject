using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CleanProject.Service.Interfaces;
using CleanProject.Service.Models;

namespace CleanProject.Service
{
    public class CleanProjectService : ICleanProjectService
    {
        public const string TITLE = "Clean Project";
        public const string DESCRIPTION = "Cleans binaries, test results and other debris from your project";

        private readonly INotificationHelper _notificationHelper;
        private readonly IZipHelper _zipHelper;
        private readonly ISolutionCleaner _solutionCleaner;

        public CleanProjectService(INotificationHelper notificationHelper, IZipHelper zipHelper,
            ISolutionCleaner solutionCleaner)
        {
            this._notificationHelper = notificationHelper;
            this._zipHelper = zipHelper;
            this._solutionCleaner = solutionCleaner;
        }

        public bool Run(CleanRequest request)
        {
            ShowNotifications(request);

            try
            {
                IEnumerable<SolutionInfo> directories = this._solutionCleaner.GetDirectories(request.Directories, request.ZipProject);
                this._solutionCleaner.CleanDirectories(directories, request.AdditionalOptions, request.RemoveSourceControl);

                if (request.ZipProject)
                {
                    this._zipHelper.ZipDirectories(directories, request.ZipDirectory);
                }
            }
            catch (ApplicationException exception)
            {
                this._notificationHelper.WriteColorMessage(ConsoleColor.Red, exception.Message);
                return false;
            }

            this._notificationHelper.WriteMessage("Cleaning complete.");

            return true;
        }

        private void ShowNotifications(CleanRequest request)
        {
            string version = Assembly.GetEntryAssembly().GetName().Version.ToString(3);
            this._notificationHelper.WriteMessage($"{TITLE} {version} - {DESCRIPTION}");

            if (request.ZipProject)
            {
                this._notificationHelper.WriteVerboseMessage("Will copy to a temporary directory, clean and zip the project");
            }

            if (request.RemoveSourceControl)
            {
                this._notificationHelper.WriteVerboseMessage("Will remove source control bindings from projects");
            }

            if (!request.QuietMode)
            {
                this._notificationHelper.WriteMessage("Will clean the following directories");
                foreach (var directory in request.Directories)
                {
                    this._notificationHelper.WriteColorMessage(ConsoleColor.Yellow, directory);
                }
            }
        }
    }
}
