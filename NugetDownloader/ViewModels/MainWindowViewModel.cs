using NugetDownloader.Events;
using NugetDownloader.Models;
using NugetDownloader.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NugetDownloader.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly INugetService _nugetService;
        private readonly IEventAggregator _eventAggregator;
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

        private string _outputText;
        public string OutputText
        {
            get => _outputText;
            set => SetProperty(ref _outputText, value);
        }

        private ObservableCollection<PackageInfo> _packages;
        public ObservableCollection<PackageInfo> Packages
        {
            get => _packages;
            set => SetProperty(ref _packages, value);
        }

        private PackageInfo _selectedPackage;
        public PackageInfo SelectedPackage
        {
            get => _selectedPackage;
            set => SetProperty(ref _selectedPackage, value);
        }

        public DelegateCommand SearchCommand { get; private set; }
        public DelegateCommand DownloadCommand { get; private set; }


        public MainWindowViewModel(INugetService nugetService, IEventAggregator eventAggregator)
        {
            _nugetService = nugetService;
            _eventAggregator = eventAggregator;
            Packages = new ObservableCollection<PackageInfo>();

            SearchCommand = new DelegateCommand(() => SearchPackage());
            DownloadCommand = new DelegateCommand(() => DownloadPackage());

            _eventAggregator.GetEvent<PackageDownloadedEvent>().Subscribe((x) => AddToOutput(x));
        }

        private async void SearchPackage()
        {
            AddToOutput("Searching package...");
            Packages.Clear();

            var packages = await _nugetService.Search(SearchTerm);

            foreach (var p in packages)
            {
                Packages.Add(p);
            }
        }

        private async Task DownloadPackage()
        {
            if (SelectedPackage != null)
            {
                AddToOutput($"Downloading {SelectedPackage.Name}");
                await _nugetService.Download(SelectedPackage);
            }
        }

        private void AddToOutput(string message)
        {
            OutputText = string.Concat(OutputText, message, Environment.NewLine);
        }


    }
}
