using NugetDownloader.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NugetDownloader.Services
{
    public interface INugetService
    {
        Task<IList<PackageInfo>> Search(string name);
        PackageInfo Download(string name, string version = "");
    }
}
