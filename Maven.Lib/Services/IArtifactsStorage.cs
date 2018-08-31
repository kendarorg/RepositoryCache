using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories.Repositories;
using Ioc;
using MavenProtocol.Apis;

namespace Maven.Services
{
    public interface IArtifactsStorage : ISingleton
    {
        byte[] Load(RepositoryEntity repo, MavenIndex index);
        void Save(RepositoryEntity repo, MavenIndex index, byte[] data);
        List<string> GetSubDir(RepositoryEntity repo, MavenIndex index);
    }
}
