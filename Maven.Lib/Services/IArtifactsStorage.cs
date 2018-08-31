using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories.Repositories;
using Ioc;
using MavenProtocol.Apis;

namespace Nuget.Services
{
    public interface IArtifactsStorage : ISingleton
    {
        byte[] Load(RepositoryEntity repo, MavenIndex index);
        void Save(RepositoryEntity repo, MavenIndex index, byte[] data);
    }
}
