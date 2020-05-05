using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CleanProject.Service.Interfaces;
using CleanProject.Service.Models;
using SimpleInjector;

namespace CleanProject.UI
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var container = Bootstrap();

            // Any additional other configuration, e.g. of your desired MVVM toolkit.

            RunApplication(container);
        }

        private static Container Bootstrap()
        {
            // Create the container as usual.
            var container = new Container();

            // Register your types, for instance:
            string currentFolder = AppDomain.CurrentDomain.BaseDirectory;
            var assemblies = new DirectoryInfo(currentFolder).GetFiles("CleanProject*")
                                        .Where(f => f.Extension.ToLower() == ".dll")
                                        .Select(f => Assembly.Load(AssemblyName.GetAssemblyName(f.FullName)))
                                        .ToList();

            container.RegisterPackages(assemblies);

            container.Register<IOptions>(() => new Options(new[] { "packages", ".vs" }, null, null), Lifestyle.Singleton);
            container.Register<INotificationHelper, Notificator>(Lifestyle.Singleton);

            // Register your windows and view models:
            container.Register<MainWindow>();

            container.Verify();

            return container;
        }

        private static void RunApplication(Container container)
        {
            try
            {
                var app = new App();
                //app.InitializeComponent();

                var mainWindow = container.GetInstance<MainWindow>();
                app.Run(mainWindow);
            }
            catch (Exception ex)
            {
                //Log the exception and exit
            }
        }
    }
}
