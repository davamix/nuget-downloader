using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NugetDownloader.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NugetDownloader.Services
{
    public class NugetService : INugetService
    {
        public async Task<IList<PackageInfo>> Search(string name)
        {
            IList<PackageInfo> packages = new List<PackageInfo>();

            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            PackageSearchResource resource = await repository.GetResourceAsync<PackageSearchResource>();
            SearchFilter filter = new SearchFilter(includePrerelease: false);

            IEnumerable<IPackageSearchMetadata> results = await resource.SearchAsync(
                name, filter, skip: 0, take: 20, logger, cancellationToken);

            foreach(IPackageSearchMetadata result in results)
            {
                packages.Add(new PackageInfo(result.Identity.Id, result.Identity.Version.ToString(), result.Description));
            }

            return packages;
        }

        public PackageInfo Download(string name, string version = "") => throw new NotImplementedException();
        
    }
}
