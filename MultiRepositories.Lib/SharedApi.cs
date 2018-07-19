using MultiRepositories.Commons;
using MultiRepositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRepositories
{
    public class SharedApi : IPackagesRepository
    {
        private AppProperties _applicationPropertes;
        private IAvailableRepositoriesRepository _availableRepositories;
        private List<IPackagesRepository> _packagesRepositories;

        public SharedApi(
            AppProperties appProperties,
            IAvailableRepositoriesRepository availableRepositories,
            List<IPackagesRepository> packagesRepositories
            )
        {
            _applicationPropertes = appProperties;
            _availableRepositories = availableRepositories;
            _packagesRepositories = packagesRepositories;
        }

        public void Initialize(IRepositoryServiceProvider repositoryServiceProvider)
        {
            repositoryServiceProvider.RegisterApi(new ServicesIndexApi(_applicationPropertes, _availableRepositories));

        }
    }
}