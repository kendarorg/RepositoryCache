using MultiRepositories.Repositories;
using Ioc;
using MavenProtocol.Apis;
using System.Collections.Generic;

namespace Maven.Services
{
    public interface IArtifactsStorage : ISingleton
    {
        byte[] Load(RepositoryEntity repo, string[] group, string artifactId, string version, string classifier, string type);
        void Write(RepositoryEntity repo, string[] group, string artifactId, string version, string classifier, string type, byte[] data);
        List<string> ListChildren(RepositoryEntity repo, string[] group);
    }
}
