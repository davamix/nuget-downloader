using System;
using System.Collections.Generic;
using System.Text;

namespace NugetDownloader.Models
{
    public record PackageInfo(string Name, string Version, string Description, IList<PackageInfoDependency> Dependencies);
}
