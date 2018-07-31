using MultiRepositories.Repositories;
using Ioc;
using MavenProtocol.Apis;

namespace Maven.Services
{
    public interface IPackagesStorage : ISingleton
    {
        /*byte[] ReadPackage(RepositoryEntity repo, MavenVersionedArtifact item);
        string ReadPackageMetadata(RepositoryEntity repo, MavenVersionedArtifact item);
        string ReadPom(RepositoryEntity repo, MavenVersionedArtifact item);*/
    }
}
