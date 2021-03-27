using NugetDownloader.Services;
using NugetDownloader.Views;
using Prism.Ioc;
using System.Windows;

namespace NugetDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<INugetService, NugetService>();

        }
    }
}
