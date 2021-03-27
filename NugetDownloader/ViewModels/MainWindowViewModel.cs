using NugetDownloader.Models;
using NugetDownloader.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace NugetDownloader.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly INugetService _nugetService;

        private string _title = "Nuget Packages Downloader";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _seachTerm;
        public string SearchTerm
        {
            get => _seachTerm;
            set => SetProperty(ref _seachTerm, value);
        }

        private ObservableCollection<PackageInfo> _packages;
        public ObservableCollection<PackageInfo> Packages
        {
            get => _packages;
            set => SetProperty(ref _packages, value);
        }

        public DelegateCommand SearchCommand { get; private set; }


        public MainWindowViewModel(INugetService nugetService)
        {
            _nugetService = nugetService;

            Packages = new ObservableCollection<PackageInfo>();

            SearchCommand = new DelegateCommand(() => SearchPackage());
        }

        private async void SearchPackage()
        {
            Packages.Clear();

            var packages = await _nugetService.Search(SearchTerm);

            foreach (var p in packages)
            {
                Packages.Add(p);
            }
        }


    }
}
