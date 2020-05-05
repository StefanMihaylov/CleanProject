using System.Collections.Generic;

namespace CleanProject.Service.Interfaces
{
    public interface IDirectoryHelper
    {
        void CopyDirectory(string source, string destination, bool subdirs, bool removeIfExists);

        void DeleteDirectory(string directory, bool skipNotifications);

        void RemoveSubDirectories(string directory, IEnumerable<string> searchPatterns, bool skipNotifications);
    }
}