using Maven.Services;
using MavenProtocol.Apis;
using MultiRepositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Apis
{
    public class MavenReadService //: IMavenRead
    {
        /*private IPackagesStorage _packagesStorage;
        private readonly IRepositoryEntitiesRepository _repositories;

        public MavenReadService(IPackagesStorage packagesStorage, IRepositoryEntitiesRepository repositories)
        {
            _packagesStorage = packagesStorage;
            _repositories = repositories;
        }

        public byte[] ReadPackage(Guid repoId, MavenVersionedArtifact item)
        {
            var repo = _repositories.GetById(repoId);
            return _packagesStorage.ReadPackage(repo, item);
        }

        public string ReadMetadata(Guid repoId, MavenPackageMetadata item)
        {
            throw new NotImplementedException();
        }

        public string ReadPom(Guid repoId, MavenVersionedArtifact item)
        {
            var repo = _repositories.GetById(repoId);
            return _packagesStorage.ReadPom(repo, item);
        }*/
    }
}
