using NugetDownloader.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NugetDownloader.ViewModels.Dialogs
{
    public class DependenciesDialogViewModel : BindableBase, IDialogAware
    {
        private string _packageName;
        public string PackageName
        {
            get => _packageName;
            set => SetProperty(ref _packageName, value);
        }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        private ObservableCollection<PackageInfoDependency> _dependencies;

        public event Action<IDialogResult> RequestClose;

        public ObservableCollection<PackageInfoDependency> Dependencies
        {
            get => _dependencies;
            set => SetProperty(ref _dependencies, value);
        }

        public string Title => "Dependencies";

        public DependenciesDialogViewModel()
        {
            Dependencies = new ObservableCollection<PackageInfoDependency>();
            OkCommand = new DelegateCommand(()=>CloseDialog(true));
            CancelCommand = new DelegateCommand(() => CloseDialog(false));
        }

        public bool CanCloseDialog() => true;
        public void OnDialogClosed() { }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("dependencies"))
            {
                parameters.TryGetValue<List<PackageInfoDependency>>("dependencies", out var dependencies);

                if (dependencies != null)
                    LoadDependencies(dependencies);
            }
        }

        private void CloseDialog(bool result)
        {
            var buttonResult = ButtonResult.None;

            if (result)
                buttonResult = ButtonResult.OK;
            else if (!result)
                buttonResult = ButtonResult.Cancel;

            RequestClose?.Invoke(new DialogResult(buttonResult));
        }

        private void LoadDependencies(List<PackageInfoDependency> dependencies)
        {
            Dependencies.Clear();

            foreach (var d in dependencies)
            {
                Dependencies.Add(d);
            }
        }
    }
}
