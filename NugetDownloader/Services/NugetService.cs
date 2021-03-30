using Microsoft.Extensions.Configuration;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NugetDownloader.Events;
using NugetDownloader.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NugetDownloader.Services
{
    public class NugetService : INugetService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IConfiguration _configuration;

        public NugetService(IEventAggregator eventAggregator, IConfiguration configuration)
        {
            _eventAggregator = eventAggregator;
            _configuration = configuration;
        }

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

        public async Task Download(PackageInfo package)
        {
            var logger = NullLogger.Instance;
            var cancellationToken = CancellationToken.None;

            var cacheContext = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            var resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            var version = new NuGetVersion(package.Version);

            using(MemoryStream packageStream = new MemoryStream())
            {
                await resource.CopyNupkgToStreamAsync(
                    package.Name,
                    version,
                    packageStream,
                    cacheContext,
                    logger,
                    cancellationToken);

                using (var packageReader = new PackageArchiveReader(packageStream))
                {
                    var nuspecReader = await packageReader.GetNuspecReaderAsync(cancellationToken);

                    _eventAggregator.GetEvent<PackageDownloadedEvent>().Publish(nuspecReader.GetTags());
                    _eventAggregator.GetEvent<PackageDownloadedEvent>().Publish(nuspecReader.GetDescription());
                }
            }

        }
        
    }
}
