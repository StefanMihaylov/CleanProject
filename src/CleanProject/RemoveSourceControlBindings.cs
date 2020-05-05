using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace CleanProject
{
    internal class RemoveSourceControlBindings
    {
        internal RemoveSourceControlBindings()
        {
            recursiveSearch = new RecursiveSearchHelper();
        }

        /// <summary>
        ///     Get or Set the temporal directory for process proposes
        /// </summary>
        internal string ProcessDirectory { get; set; }

        private const string EndGlobalSection = "EndGlobalSection";

        private const string GlobalSection = "GlobalSection(TeamFoundationVersionControl)";

        private const string ProjectPattern = "*.*proj";

        private const string SolutionPattern = "*.sln";

        private readonly string[] bindingPattern = {"*.vssscc", "*.vspscc", "*.scc"};

        private readonly RecursiveSearchHelper recursiveSearch;

        internal static void Clean(string directory)
        {
            var scCleaner = new RemoveSourceControlBindings {ProcessDirectory = directory};
            scCleaner.Execute();
        }

        /// <summary>
        ///     Remove Source Control Bindings according to a specified binding pattern.
        /// </summary>
        internal virtual void Execute()
        {
            DeleteSourceControlFiles();
            CleanSolutions();
            CleanProjects();
        }

        protected virtual void CleanProjects()
        {
            var projects = recursiveSearch.GetFiles(ProcessDirectory, ProjectPattern);

            foreach (var projectFile in projects)
            {
                Program.WriteVerboseMessage("Cleaning project file  {0}", projectFile);

                var projectDocument = XDocument.Load(projectFile);

                foreach (var item in projectDocument.Elements())
                {
                    RemoveSccElements(item);
                }
                var readOnly = projectFile.TurnOffReadOnlyFlag();

                projectDocument.Save(projectFile);

                if (readOnly)
                {
                    projectFile.TurnOnReadOnlyFlag();
                }
            }
        }

        protected virtual void CleanSolutions()
        {
            var solutions = recursiveSearch.GetFiles(ProcessDirectory, SolutionPattern);

            foreach (var solutionFile in solutions)
            {
                Program.WriteVerboseMessage("Cleaning solution {0}", solutionFile);

                var solutionText = File.ReadAllText(solutionFile);

                if (solutionText.Contains(GlobalSection))
                {
                    var tfs = GetTfsGlobalSection(solutionText);

                    var readOnly = solutionFile.TurnOffReadOnlyFlag();

                    File.WriteAllText(solutionFile, solutionText.Replace(tfs, null), Encoding.UTF8);

                    if (readOnly)
                    {
                        solutionFile.TurnOnReadOnlyFlag();
                    }
                }
            }
        }

        protected virtual void DeleteSourceControlFiles()
        {
            var files = new List<string>();

            foreach (var pattern in bindingPattern)
            {
                files.AddRange(recursiveSearch.GetFiles(ProcessDirectory, pattern));
            }

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                Program.WriteVerboseMessage("Deleting file {0}", file);
                File.Delete(file);
            }
        }

        private static string GetTfsGlobalSection(string solutionText)
        {
            var globalTfsSection = solutionText.Substring(solutionText.IndexOf(GlobalSection, StringComparison.OrdinalIgnoreCase));
            globalTfsSection = globalTfsSection.Substring(0, (globalTfsSection.IndexOf(EndGlobalSection, StringComparison.OrdinalIgnoreCase) + EndGlobalSection.Length));

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
    }
}