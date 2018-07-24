using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Repositories
{
    public interface IPackagesRepository : IRepository<PackageEntity>
    {
        PackageEntity GetByPackage(Guid repoId, string lowerId, string lowerVersion);
        PackageEntity GetByIdVersion(Guid repoId, string lowerIdlowerVersion);
        IEnumerable<PackageEntity> GetByIdVersions(Guid repoId,string lowerId, string[] lowerVersions);
        IEnumerable<PackageEntity> GetByPackageId(Guid repoId,string id);
    }
}
