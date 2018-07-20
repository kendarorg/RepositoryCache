
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;
using Repositories;

namespace MultiRepositories.Repositories
{
    public class AvailableRepositoriesRepository :InMemoryRepository<RepositoryEntity>, IRepositoryEntitiesRepository
    {
        public AvailableRepositoriesRepository(AppProperties properties) : 
            base(properties)
        {
        }

        public RepositoryEntity GetByName(string repoPrefix)
        {
            return GetAll().First(v => string.Compare(v.Prefix, repoPrefix, true) == 0);
        }

        public IEnumerable<RepositoryEntity> GetByType(string type)
        {
            throw new NotImplementedException();
        }
    }
}
