using Maven.Repositories;
using Maven.Services;
using MavenProtocol;
using MavenProtocol.Apis;
using MavenProtocol.Apis.Browse;
using MultiRepositories.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Maven.Apis
{
    public class MavenExploreService : IMavenExploreService
    {
        private readonly IArtifactsStorage _artifactsStorage;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IMavenArtifactsService _mavenArtifactsService;
        private readonly IMavenArtifactsRepository _mavenArtifactsRepository;
        private readonly IMavenSearchRepository _mavenSearchRepository;

        public MavenExploreService(IArtifactsStorage mavenTreeRepository,
            IRepositoryEntitiesRepository repositoryEntitiesRepository,
            IMavenArtifactsService mavenArtService,
            IMavenArtifactsRepository mavenArtifactsRepository,
            IMavenSearchRepository mavenSearchRepository)
        {
            this._artifactsStorage = mavenTreeRepository;
            this._repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._mavenArtifactsService = mavenArtService;
            this._mavenArtifactsRepository = mavenArtifactsRepository;
            _mavenSearchRepository = mavenSearchRepository;
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
                                return null;
                            }
                            else
                            {
                                baseUrl = null;
                                result.Content = _mavenArtifactsService.ReadArtifact(repoId,explore);
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(explore.Version))
                        {
                            baseUrl += "/" + explore.Version;
                            if (!string.IsNullOrWhiteSpace(explore.Filename))
                            {
                                baseUrl = null;
                                result.Content = _mavenArtifactsService.ReadArtifact(repoId, explore);
                            }
                            else if (!string.IsNullOrWhiteSpace(explore.Checksum))
                            {
                                baseUrl = null;
                                result.Content = Encoding.UTF8.GetBytes(_mavenArtifactsService.ReadChecksum(repoId, explore));
                            }
                            else
                            {

                                result.Children = GetListOfFilesForVersion(explore, repo);
                            }
                        }
                        else
                        {
                            foreach(var item in _mavenSearchRepository.GetByArtifactId(repoId, 
                                explore.ArtifactId,string.Join(".", explore.Group)))
                            {
                                result.Children.Add(item.Version);
                            }
                            result.Children.Add("maven-metadata.xml");
                        }
                    }
                    else
                    {
                        result.Children = _artifactsStorage.ListChildren(repo, explore.Group);
                    }
                }
                else
                {
                    result.Children = _artifactsStorage.ListChildren(repo, explore.Group);
                }
            }
            result.Base = baseUrl;
            if (result.Children == null || !result.Children.Any() || result.Content == null)
            {
                result.Children = _artifactsStorage.ListChildren(repo, new string[] { });
            }
            return result;
        }

        private List<string> GetListOfFilesForVersion(MavenIndex explore, RepositoryEntity repo)
        {
            var resulta = new List<string>();
            foreach (var a in _mavenArtifactsRepository.GetById(repo.Id, explore.Group, explore.ArtifactId, explore.Version))
            {
                var res = a.ArtifactId + "-" + a.Version;
                if (!string.IsNullOrWhiteSpace(a.Classifier))
                {
                    res += a.Classifier;
                }
                res += "." + a.Type;
                resulta.Add(res);
                if (!string.IsNullOrWhiteSpace(a.Asc))
                {
                    resulta.Add(res + ".asc");
                }
                if (!string.IsNullOrWhiteSpace(a.Md5))
                {
                    resulta.Add(res + ".md5");
                }
                if (!string.IsNullOrWhiteSpace(a.Sha1))
                {
                    resulta.Add(res + ".sha1");
                }
            }

            return resulta;
        }       
    }
}
