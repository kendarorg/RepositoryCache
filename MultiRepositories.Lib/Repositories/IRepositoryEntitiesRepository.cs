using System.Collections.Generic;
using Repositories;

namespace MultiRepositories.Repositories
{
    public interface IRepositoryEntitiesRepository : IRepository<RepositoryEntity>
    {
        RepositoryEntity GetByName(string repoPrefix);
        IEnumerable<RepositoryEntity> GetByType(string type);
    }
}