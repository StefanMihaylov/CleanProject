using CleanProject.Service.Helpers;
using CleanProject.Service.Interfaces;
using CleanProject.Service.Processors;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace CleanProject.Service
{
    public class CleanProjectServiceModule : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.Register<IDirectoryHelper, DirectoryHelper>(Lifestyle.Singleton);
            container.Register<IFileHelper, FileHelper>(Lifestyle.Singleton);
            container.Register<IRemoveSourceControlBindings, RemoveSourceControlBindings>(Lifestyle.Singleton);

            container.Register<ISolutionCleaner, SolutionCleaner>(Lifestyle.Singleton);
            container.Register<IZipHelper, ZipHelper>(Lifestyle.Singleton);

            container.Register<ICleanProjectService, CleanProjectService>(Lifestyle.Singleton);
        }
    }
}
