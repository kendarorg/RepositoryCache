using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;

namespace Nuget.Repositories
{
    public class NugetAssembliesRepository : InMemoryRepository<NugetAssemblyGroup>, INugetAssembliesRepository
    {
        public NugetAssembliesRepository(AppProperties properties) : base(properties)
        {
        }

        public IEnumerable<NugetAssemblyGroup> GetGroups(Guid repoId, string idLower, string versionLower)
        {
            return GetAll().Where(a => a.RepositoryId == repoId &&
               a.OwnerPackageId == idLower && a.OwnerVersion == versionLower);
        }
    }
}
