using System.Collections.Generic;
using Repositories;

namespace MultiRepositories.Repositories
{
    public interface IUsersRepository : IRepository<UserEntity>
    {
    }
}