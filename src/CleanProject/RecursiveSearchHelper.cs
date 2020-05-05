using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CleanProject
{
    internal class RecursiveSearchHelper
    {
        internal RecursiveSearchHelper()
        {
            fileList = new List<string>();
            excludeList = new List<string>();
        }

        private readonly List<string> excludeList;

        private readonly List<string> fileList;

        internal string[] GetFiles(string initialDirectory, string filePattern)
        {
            fileList.Clear();

            Search(initialDirectory, filePattern);

            return fileList.ToArray();
        }

        internal string[] GetFiles(string initialDirectory, string[] filePatterns, string[] excludePatterns)
        {
            fileList.Clear();
            excludeList.Clear();

            foreach (var filePattern in filePatterns)
            {
                Search(initialDirectory, filePattern);
            }

            if (excludePatterns != null)
            {
                foreach (var excludePattern in excludePatterns)
                {
                    SearchExclude(initialDirectory, excludePattern);
                }
            }

            foreach (var file in excludeList)
            {
                fileList.RemoveAll(s => s == file);
            }

            return fileList.ToArray();
        }

        private void Search(string initialDirectory, string filePattern)
        {
            foreach (var file in Directory.GetFiles(initialDirectory, filePattern).Where(file => !fileList.Contains(file)))
            {
                fileList.Add(file);
            }

            foreach (var item in Directory.GetDirectories(initialDirectory))
            {
                Search(item, filePattern);
            }
        }

        private void SearchExclude(string initialDirectory, string excludePattern)
        {
            foreach (var file in Directory.GetFiles(initialDirectory, excludePattern).Where(file => !excludeList.Contains(file)))
            {
                excludeList.Add(file);
            }

            foreach (var item in Directory.GetDirectories(initialDirectory))
            {
                SearchExclude(item, excludePattern);
            }
        }
    }
}