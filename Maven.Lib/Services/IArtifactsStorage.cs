using MultiRepositories.Repositories;
using Ioc;
using MavenProtocol.Apis;

namespace Maven.Services
{
    public interface IArtifactsStorage : ISingleton
    {
        byte[] Load(RepositoryEntity repo, string[] group, string artifactId, string version, string classifier, string type);
    }
}
