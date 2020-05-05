using System.Collections.Generic;
using CleanProject.Service.Interfaces;
using CleanProject.Service.Models;

namespace CleanProject.Service.Interfaces
{
    public interface ISolutionCleaner
    {
        void CleanDirectories(IEnumerable<SolutionInfo> directories, IOptions additionalOptions, bool removeSourceControl);

        IEnumerable<SolutionInfo> GetDirectories(IEnumerable<string> directories, bool zipProject);
    }
}