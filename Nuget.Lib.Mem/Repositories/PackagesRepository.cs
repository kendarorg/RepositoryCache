using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;

namespace Nuget.Repositories
{
    public class PackagesRepository : InMemoryRepository<PackageEntity>, IPackagesRepository
    {
        public PackagesRepository(AppProperties properties) : base(properties)
        {
        }

        public PackageEntity GetByIdVersion(Guid repoId, string lowerIdlowerVersion)
        {
            return GetAll().FirstOrDefault(a => a.RepositoryId == repoId && a.PackageIdAndVersion == lowerIdlowerVersion);
        }

        public IEnumerable<PackageEntity> GetByIdVersions(Guid repoId, string lowerId, string[] lowerVersions)
        {
            return GetAll().Where(a => a.RepositoryId == repoId &&
               a.PackageId == lowerId &&
              lowerVersions.Contains(a.Version));
        }

        public PackageEntity GetByPackage(Guid repoId, string lowerId, string lowerVersion)
        {
            return GetAll().FirstOrDefault(a => a.RepositoryId == repoId &&
                a.PackageId == lowerId &&
                a.Version == lowerVersion);
        }
    }
}
