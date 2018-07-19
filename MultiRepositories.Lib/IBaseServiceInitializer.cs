using MultiRepositories.Repositories;
using MultiRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRepositories
{
    public interface IBaseServiceInitializer
    {
        void Initialize(IRepositoryServiceProvider repositoryServiceProvider, AppProperties appProperties, AvailableRepositoriesRepository availableRepositoriesRepository);
    }
}
