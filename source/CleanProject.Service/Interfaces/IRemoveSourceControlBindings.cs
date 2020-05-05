using System.Collections.Generic;

namespace CleanProject.Service.Interfaces
{
    public interface IRemoveSourceControlBindings
    {
        void Clean(string directory, IEnumerable<string> bindingPattern);
    }
}