using System.Collections.Generic;
using CleanProject.Service.Models;

namespace CleanProject.Service.Interfaces
{
    public interface IZipHelper
    {
        void ZipDirectories(IEnumerable<SolutionInfo> directoryInfos, string zipDirectory);
    }
}