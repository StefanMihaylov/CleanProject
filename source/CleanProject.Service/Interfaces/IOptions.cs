using System.Collections.Generic;

namespace CleanProject.Service.Interfaces
{
    public interface IOptions
    {
        IEnumerable<string> RemoveDirectories { get; }

        IEnumerable<string> RemoveFiles { get; }

        IEnumerable<string> RemoreSourceControl { get; }
    }
}