using NugetDownloader.Events;
using NugetDownloader.Models;
using NugetDownloader.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace NugetDownloader.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly INugetService _nugetService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;
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


        public MainWindowViewModel(INugetService nugetService, IEventAggregator eventAggregator, IDialogService dialogService)
        {
            _nugetService = nugetService;
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
            Packages = new ObservableCollection<PackageInfo>();

            SearchCommand = new DelegateCommand(() => SearchPackage());
            DownloadCommand = new DelegateCommand(() => DownloadPackage());

            _eventAggregator.GetEvent<PackageDownloadedEvent>().Subscribe((x) => AddToOutput(x));
            _eventAggregator.GetEvent<PackageDependenciesEvent>().Subscribe((x) => ShowDependencies(x));
        }

        private async void SearchPackage()
        {
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                AddToOutput($"Searching package {SearchTerm}...");
                Packages.Clear();

                var packages = await _nugetService.Search(SearchTerm);

                foreach (var p in packages)
                {
                    Packages.Add(p);
                }
            }

        }

        private async Task DownloadPackage()
        {
            if (SelectedPackage != null)
            {
                AddToOutput($"Downloading {SelectedPackage.Name}");
                //await _nugetService.Download(SelectedPackage);
                try
                {

                    var dependencies = await _nugetService.Download(SelectedPackage);

                    if (dependencies.Any())
                    {
                        ShowDependencies(dependencies);
                    }

                    //var dependencies = await _nugetService.SearchDependencies(SelectedPackage);

                    
                }
                catch (Exception e)
                {
                    AddToOutput($"[ERROR] {e.Message}");

                }

            }
        }

        private void AddToOutput(string message)
        {
            OutputText = string.Concat(OutputText, message, Environment.NewLine);
        }

        private void ShowDependencies(List<PackageInfoDependency> dependencies)
        {
            var p = new DialogParameters()
            {
                {"dependencies", dependencies }
            };
            
            _dialogService.ShowDialog("DependenciesDialog", p, r =>
            {
                switch (r.Result)
                {
                    case ButtonResult.OK:
                        AddToOutput("OK");
                        break;
                    case ButtonResult.Cancel:
                        AddToOutput("Cancel");
                        break;
                    default:
                        AddToOutput("Nothing");
                        break;
                }
            });
        }


    }
}
