using Ioc;
using MultiRepositories.Repositories;
using Nuget.Repositories;
using NugetProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Apis
{
    public class NugetAutocompleteService : IAutocompleteService, ISingleton
    {


        private readonly IServicesMapper _servicesMapper;
        private readonly IQueryRepository _queryRepository;
        private readonly IPackagesRepository _packagesRepository;

        public NugetAutocompleteService(
            IQueryRepository queryRepository,
            IServicesMapper servicesMapper,
            IPackagesRepository packagesRepository)
        {
            _servicesMapper = servicesMapper;
            _queryRepository = queryRepository;
            _packagesRepository = packagesRepository;
        }

        public AutocompleteResult Query(Guid repoId, QueryModel query)
        {
            var packages = new List<string>();

            var lastCommitId = Guid.NewGuid();
            var lastTimestamp = DateTime.MinValue;

            foreach (var item in _queryRepository.Query(repoId, query))
            {
                lastCommitId = item.CommitId;
                lastTimestamp = item.CommitTimestamp > lastTimestamp ? item.CommitTimestamp : lastTimestamp;
                if (query.PreRelease && item.HasPreRelease)
                {
                    packages.Add(item.PackageId);
                }
                else if (item.HasRelease)
                {
                    packages.Add(item.PackageId);
                }
            }

            return new AutocompleteResult(
                new AutocompleteContext(_servicesMapper.From(repoId, "*Schema")),
                packages.Count, lastTimestamp, lastCommitId.ToString(),
                packages
                );
        }

        public AutocompleteResult QueryByPackage(Guid repoId, string id, bool prerelease = true, string semVerLevel = "1.0.0")
        {
            var lastCommitId = Guid.NewGuid();
            var lastTimestamp = DateTime.MinValue;
            var versions = new List<string>();
            foreach (var item in _packagesRepository.GetByPackageId(repoId, id))
            {
                lastCommitId = item.CommitId;
                lastTimestamp = item.CommitTimestamp > lastTimestamp ? item.CommitTimestamp : lastTimestamp;

                var version = SemVer.SemVersion.Parse(item.Version);
                if (!prerelease && !string.IsNullOrWhiteSpace(version.Prerelease))
                {
                    continue;
                }
                versions.Add(item.Version);
            }

            return new AutocompleteResult(
                new AutocompleteContext(_servicesMapper.From(repoId, "*Schema")),
                versions.Count, lastTimestamp, lastCommitId.ToString(),
                versions
                );
        }
    }
}
