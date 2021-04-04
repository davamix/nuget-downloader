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
        Task<List<PackageInfoDependency>> Download(PackageInfo package);
        Task<IList<PackageInfoDependency>> SearchDependencies(PackageInfo package);
    }
}
