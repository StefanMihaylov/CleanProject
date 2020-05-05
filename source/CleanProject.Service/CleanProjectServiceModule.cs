using CleanProject.Service.Helpers;
using CleanProject.Service.Interfaces;
using CleanProject.Service.Processors;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace CleanProject.Service
{
    class CleanProjectServiceModule : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.Register<IDirectoryHelper, DirectoryHelper>(Lifestyle.Scoped);
            container.Register<IFileHelper, FileHelper>(Lifestyle.Scoped);
            container.Register<IRemoveSourceControlBindings, RemoveSourceControlBindings>(Lifestyle.Scoped);

            container.Register<ISolutionCleaner, SolutionCleaner>(Lifestyle.Scoped);
            container.Register<IZipHelper, ZipHelper>(Lifestyle.Scoped);

            container.Register<ICleanProjectService, CleanProjectService>(Lifestyle.Scoped);
        }
    }
}
