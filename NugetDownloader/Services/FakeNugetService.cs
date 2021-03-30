using NugetDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetDownloader.Services
{
    public class FakeNugetService : INugetService
    {
        public Task Download(PackageInfo package) => throw new NotImplementedException();
        public async Task<IList<PackageInfo>> Search(string name)
        {
            return await Task.FromResult(new List<PackageInfo>()
            {
                new PackageInfo("Package A", "3.2.5", "Description package A"),
                new PackageInfo("Package B", "3.6.5", "Description package B"),
                new PackageInfo("Package C", "6.2.5", "Description package C"),
                new PackageInfo("Package D", "3.2.5", "Description package D"),
            });
        }
    }
}
