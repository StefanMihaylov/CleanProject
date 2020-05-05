using System.Collections.Generic;

namespace CleanProject.Service.Interfaces
{
    public interface IDirectoryHelper
    {
        void CopyDirectory(string source, string destination, bool subdirs, bool removeIfExists);

        void Delete(string directory);

        void RemoveSubDirectories(string directory, IEnumerable<string> searchPatterns);
    }
}