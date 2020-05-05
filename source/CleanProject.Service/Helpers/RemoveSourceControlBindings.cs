using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CleanProject.Service.Interfaces;

namespace CleanProject.Service.Helpers
{
    public class RemoveSourceControlBindings : IRemoveSourceControlBindings
    {
        private const string EndGlobalSection = "EndGlobalSection";
        private const string GlobalSection = "GlobalSection(TeamFoundationVersionControl)";
        private const string ProjectPattern = "*.*proj";
        private const string SolutionPattern = "*.sln";

        private readonly INotificationHelper _notificationHelper;
        private readonly IFileHelper _fileHelper;

        public RemoveSourceControlBindings(INotificationHelper notificationHelper, IFileHelper fileHelper)
        {
            this._notificationHelper = notificationHelper;
            this._fileHelper = fileHelper;
        }

        public void Clean(string directory, IEnumerable<string> bindingPattern)
        {
            if (bindingPattern?.Any() != true)
            {
                return;
            }

            DeleteSourceControlFiles(directory, bindingPattern);

            CleanSolutions(directory, SolutionPattern, GlobalSection, EndGlobalSection);

            CleanProjects(directory, ProjectPattern);
        }

        private void DeleteSourceControlFiles(string directory, IEnumerable<string> bindingPattern)
        {
            var files = new List<string>();

            foreach (var pattern in bindingPattern)
            {
                files.AddRange(this.GetFiles(directory, pattern));
            }

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                this._notificationHelper.WriteVerboseMessage($"Deleting file {file}");
                File.Delete(file);
            }
        }

        private void CleanSolutions(string directory, string solutionPattern, string globalSection, string endGlobalSection)
        {
            var solutions = this.GetFiles(directory, solutionPattern);
            foreach (var solutionFile in solutions)
            {
                this._notificationHelper.WriteVerboseMessage($"Cleaning solution {solutionFile}");

                var solutionText = File.ReadAllText(solutionFile);
                if (solutionText.Contains(globalSection))
                {
                    var tfs = this.GetTfsGlobalSection(solutionText, globalSection, endGlobalSection);
                    var readOnly = this._fileHelper.TurnOffReadOnlyFlag(solutionFile);

                    File.WriteAllText(solutionFile, solutionText.Replace(tfs, null), Encoding.UTF8);

                    if (readOnly)
                    {
                        this._fileHelper.TurnOnReadOnlyFlag(solutionFile);
                    }
                }
            }
        }

        private void CleanProjects(string directory, string projectPattern)
        {
            var projects = this.GetFiles(directory, projectPattern);
            foreach (var projectFile in projects)
            {
                this._notificationHelper.WriteVerboseMessage($"Cleaning project file  {projectFile}");

                var projectDocument = XDocument.Load(projectFile);

                foreach (var item in projectDocument.Elements())
                {
                    RemoveSccElements(item);
                }

                var readOnly = this._fileHelper.TurnOffReadOnlyFlag(projectFile);

                projectDocument.Save(projectFile);

                if (readOnly)
                {
                    this._fileHelper.TurnOnReadOnlyFlag(projectFile);
                }
            }
        }

        private string GetTfsGlobalSection(string solutionText, string globalSection, string endGlobalSection)
        {
            var globalTfsSection = solutionText.Substring(solutionText.IndexOf(globalSection, StringComparison.OrdinalIgnoreCase));
            globalTfsSection = globalTfsSection.Substring(0, (globalTfsSection.IndexOf(endGlobalSection, StringComparison.OrdinalIgnoreCase) + EndGlobalSection.Length));

            return globalTfsSection;
        }

        private void RemoveSccElements(XElement node)
        {
            if (node.Name.LocalName.Contains("Scc"))
            {
                node.RemoveAll();
            }
            else if (node.HasElements)
            {
                foreach (var element in node.Elements())
                {
                    RemoveSccElements(element);
                }
            }
        }

        private IEnumerable<string> GetFiles(string initialDirectory, string filePattern)
        {
            var fileList = new List<string>();

            Search(initialDirectory, filePattern, fileList);

            return fileList;
        }

        private void Search(string initialDirectory, string filePattern, IList<string> fileList)
        {
            foreach (var file in Directory.GetFiles(initialDirectory, filePattern).Where(file => !fileList.Contains(file)))
            {
                fileList.Add(file);
            }

            foreach (var item in Directory.GetDirectories(initialDirectory))
            {
                Search(item, filePattern, fileList);
            }
        }
    }
}
