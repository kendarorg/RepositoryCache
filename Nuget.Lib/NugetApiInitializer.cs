using MultiRepositories;
using MultiRepositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget
{
    public class NugetApiInitializer : IPackagesRepository
    {
        private AppProperties _applicationPropertes;
        private IRepositoryEntitiesRepository _availableRepositories;
        private List<IPackagesRepository> _packagesRepositories;

        public NugetApiInitializer(
            AppProperties appProperties,
            IRepositoryEntitiesRepository availableRepositories,
            List<IPackagesRepository> packagesRepositories
            )
        {
            _applicationPropertes = appProperties;
            _availableRepositories = availableRepositories;
            _packagesRepositories = packagesRepositories;
        }

        public void Initialize(IRepositoryServiceProvider repositoryServiceProvider)
        {
            throw new NotImplementedException();
        }
    }
}
