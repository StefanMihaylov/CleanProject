using CleanProject.Service.Models;

namespace CleanProject.Service.Interfaces
{
    public interface ICleanProjectService
    {
        bool Run(CleanRequest request);
    }
}
