using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetDownloader.Models
{
    public class PackageGroupInfo
    {
        public string TargetFrameworkName { get; set; }
        public string Version { get; set; }
        public IList<PackageInfoDependency> Packages { get; set; }

        public PackageGroupInfo()
        {
            Packages = new List<PackageInfoDependency>();
        }
    }
}
