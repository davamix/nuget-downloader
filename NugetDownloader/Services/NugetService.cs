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
                packages.Add(new PackageInfo(result.Identity.Id, result.Identity.Version.ToString(), result.Description, null));
            }

            return packages;
        }

        public async Task<List<PackageInfoDependency>> Download(PackageInfo package)
        {
            //SearchDependencies(package);

            //return;
            var packageDependencies = new List<PackageInfoDependency>();

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
                    // Download package
                    var nuspecReader = await packageReader.GetNuspecReaderAsync(cancellationToken);

                    // Get dependencies
                    var dependencies = await packageReader.GetPackageDependenciesAsync(cancellationToken);

                    foreach(var d in dependencies)
                    {
                        foreach(var dp in d.Packages)
                        {
                            packageDependencies.Add(new PackageInfoDependency(dp.Id, dp.VersionRange.MinVersion.Version.ToString(), dp.VersionRange.MaxVersion?.Version.ToString()));
                        }
                    }

                    _eventAggregator.GetEvent<PackageDownloadedEvent>().Publish(nuspecReader.GetTags());
                    _eventAggregator.GetEvent<PackageDownloadedEvent>().Publish(nuspecReader.GetDescription());
                    //_eventAggregator.GetEvent<PackageDependenciesEvent>().Publish(packageDependencies);
                }
            }

            return packageDependencies;
        }

        public async Task<IList<PackageInfoDependency>> SearchDependencies(PackageInfo package)
        {
            // https://github.com/NuGet/Samples/blob/a2e62d961af3c728d8e92caa0f67fea6e2f38fdc/NuGetProtocolSamples/Program.cs#L200

            IList<PackageInfoDependency> dependencies = new List<PackageInfoDependency>();

            //var dependencies = Search(package.Name); // pass PakcageInfo?
            var logger = NullLogger.Instance;
            var cancellationToken = CancellationToken.None;
            var cacheContext = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            var resource = await repository.GetResourceAsync<FindPackageByIdResource>();
            var version = new NuGetVersion(package.Version);

            using (MemoryStream packageStream = new MemoryStream())
            {
                await resource.GetDependencyInfoAsync(
                    package.Name,
                    version,
                    cacheContext,
                    logger,
                    cancellationToken);

                using (var packageReader = new PackageArchiveReader(packageStream))
                {
                    var nuspecReader = packageReader.NuspecReader;

                    foreach (var dg in nuspecReader.GetDependencyGroups())
                    {
                        //{dg.TargetFramework.GetShortFolderName}

                        foreach (var d in dg.Packages)
                        {
                            //{d.Id}, { d.VersionRange}
                            var dependency = new PackageInfoDependency(d.Id, d.VersionRange.MinVersion.Version.ToString(), d.VersionRange.MaxVersion.Version.ToString());
                            dependencies.Add(dependency);
                        }
                    }
                }

                return dependencies;
            }
        }
        
    }
}
