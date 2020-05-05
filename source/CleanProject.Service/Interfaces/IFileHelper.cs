using System.Collections.Generic;

namespace CleanProject.Service.Interfaces
{
    public interface IFileHelper
    {
        void DeleteFiles(string directory, IEnumerable<string> searchPatterns, bool skipNotifications);

        bool TurnOffReadOnlyFlag(string file);

        void TurnOnReadOnlyFlag(string file);
    }
}