using Maven.Repositories;
using Maven.Services;
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
        private readonly IArtifactsStorage _artifactsStorage;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IMavenArtifactsService _mavenArtifactsRepository;

        public MavenExploreService(IArtifactsStorage mavenTreeRepository,
            IRepositoryEntitiesRepository repositoryEntitiesRepository,
            IMavenArtifactsService mavenArtifactsRepository)
        {
            this._artifactsStorage = mavenTreeRepository;
            this._repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._mavenArtifactsRepository = mavenArtifactsRepository;
        }

        public ExploreResult Explore(Guid repoId, MavenIndex explore)
        {
            var repo = _repositoryEntitiesRepository.GetById(repoId);
            var result = new ExploreResult();
            var baseUrl = "/" + repo.Prefix;
            if (explore.Group != null && explore.Group.Any())
            {
                baseUrl += "/" + string.Join("/", explore.Group);
                if (!string.IsNullOrWhiteSpace(explore.ArtifactId))
                {
                    baseUrl += "/" + explore.ArtifactId;
                    if (!string.IsNullOrWhiteSpace(explore.Type))
                    {
                        if (explore.Type == "maven-metadata")
                        {
                            if (!string.IsNullOrWhiteSpace(explore.Checksum))
                            {
                                baseUrl = null;
                                result.Content = Encoding.UTF8.GetBytes(_mavenArtifactsRepository.ReadChecksum(repoId, explore));
                            }
                            else
                            {
                                baseUrl = null;
                                result.Content = _mavenArtifactsRepository.ReadArtifact(repoId, explore);
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(explore.Version))
                        {
                            baseUrl += "/" + explore.Version;
                            if (!string.IsNullOrWhiteSpace(explore.Filename))
                            {
                                baseUrl = null;
                                result.Content = _mavenArtifactsRepository.ReadArtifact(repoId, explore);
                            }
                            else if (!string.IsNullOrWhiteSpace(explore.Checksum))
                            {
                                baseUrl = null;
                                result.Content = Encoding.UTF8.GetBytes(_mavenArtifactsRepository.ReadChecksum(repoId, explore));
                            }
                            else
                            {
                                throw new Exception("list all items for given version");
                            }
                        }
                        else
                        {
                            throw new Exception("list versions list + maven-metadata");
                        }
                    }
                }
                else
                {
                    result.Children = _artifactsStorage.ListChildren(repo, explore.Group);
                }
            }
            result.Base = baseUrl;
            return result;
        }
    }
}
