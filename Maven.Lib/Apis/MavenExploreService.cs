using Maven.Repositories;
using MavenProtocol.Apis;
using MavenProtocol.Apis.Browse;
using MultiRepositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Apis
{
    public class MavenExploreService : IMavenExploreService
    {
        private readonly IMavenTreeRepository _mavenTreeRepository;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;

        public MavenExploreService(IMavenTreeRepository mavenTreeRepository,
            IRepositoryEntitiesRepository repositoryEntitiesRepository)
        {
            this._mavenTreeRepository = mavenTreeRepository;
            this._repositoryEntitiesRepository = repositoryEntitiesRepository;
        }

        public ExploreResult Explore(Guid repoId, MavenIndex explore)
        {
            var repo = _repositoryEntitiesRepository.GetById(repoId);
            var allItem = _mavenTreeRepository.GetAllChild(explore.Group, explore.ArtifactId, explore.Version);
            throw new NotImplementedException();
        }
    }
}
