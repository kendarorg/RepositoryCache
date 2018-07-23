using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories.Repositories;
using Nuget.Repositories;
using Ioc;

namespace Nuget.Services
{
    public interface IPackagesStorage : ISingleton
    {
        byte[] Load(RepositoryEntity repo, PackageEntity package);
        void Save(RepositoryEntity repo, PackageEntity package, byte[] data);
    }
}
