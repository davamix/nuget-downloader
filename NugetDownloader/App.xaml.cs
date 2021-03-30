﻿using Microsoft.Extensions.Configuration;
using NugetDownloader.Services;
using NugetDownloader.Views;
using Prism.Ioc;
using System.IO;
using System.Windows;

namespace NugetDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public IConfiguration Configuration { get; private set; }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterInstances(containerRegistry);
            RegisterServices(containerRegistry);
        }

        private void RegisterInstances(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IConfiguration>(new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                           .Build());
        }

        private void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<INugetService, NugetService>();
        }
    }
}
