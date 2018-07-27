using MultiRepositories.Repositories;
using Ioc;

namespace Maven.Services
{
    public interface IPackagesStorage : ISingleton
    {
        byte[] Load(RepositoryEntity repo, string id,string normalVersio);
        void Save(RepositoryEntity repo, string id, string normalVersio, byte[] data);
    }
}
