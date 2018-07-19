using Repositories;

namespace MultiRepositories.Repositories
{
    public interface IAvailableRepositoriesRepository:IRepository<AvailableRepositoryEntity>
    {
        AvailableRepositoryEntity GetByName(string repoPrefix);
    }
}