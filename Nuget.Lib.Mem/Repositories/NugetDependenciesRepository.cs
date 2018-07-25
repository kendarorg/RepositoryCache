using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;

namespace Nuget.Repositories
{
    public class NugetDependenciesRepository : InMemoryRepository<NugetDependency>, INugetDependenciesRepository
    {
        public NugetDependenciesRepository(AppProperties properties) : base(properties)
        {
        }

        public IEnumerable<NugetDependency> GetDependencies(Guid repoId, string idLower, string versionLower)
        {
            return GetAll().Where(a => a.RepositoryId == repoId &&
               a.OwnerPackageId == idLower && a.OwnerVersion ==versionLower);
        }
    }
}
