using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetDownloader.Models
{
    public class PackageInfoDependency
    {
        public string Id { get; set; }
        public string MinVersion { get; set; }
        public string MaxVersion { get; set; }

        public PackageInfoDependency(string id, string minVersion, string maxVersion)
        {
            Id = id;
            MinVersion = minVersion;
            MaxVersion = maxVersion;
        }
        public PackageInfoDependency()
        {

        }
    }
}
