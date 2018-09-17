using Ioc;
using MultiRepositories.Commons;
using MultiRepositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRepositories
{
    public class SharedApi : IApiProvider
    {
        private AppProperties _applicationPropertes;
        private IRepositoryEntitiesRepository _availableRepositories;

        public SharedApi(
            AppProperties appProperties,
            IRepositoryEntitiesRepository availableRepositories
            )
        {
            _applicationPropertes = appProperties;
            _availableRepositories = availableRepositories;
        }

        public void Initialize(IRepositoryServiceProvider repositoryServiceProvider)
        {
            repositoryServiceProvider.RegisterApi(new ServicesIndexApi(
                _applicationPropertes, _availableRepositories,
                "/global/index.json"));

        }
    }
}