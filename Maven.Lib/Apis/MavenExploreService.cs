using Maven.Repositories;
using Maven.Services;
using MavenProtocol;
using MavenProtocol.Apis;
using MavenProtocol.Apis.Browse;
using MultiRepositories.Repositories;
using Maven.Services;
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
        private readonly IArtifactRepository _artifactsRepository;
        private readonly IVersionedArtifactRepository _versionedArtifactsRepository;
        private readonly IVersionedClassifiersRepository _versionedClassifiersRepository;

        public MavenExploreService(IArtifactsStorage mavenTreeRepository,
            IRepositoryEntitiesRepository repositoryEntitiesRepository,
            IMavenArtifactsService mavenArtService,
            IArtifactRepository mavenArtifactsRepository,
            IVersionedArtifactRepository mavenSearchRepository,
            IVersionedClassifiersRepository versionedClassifiersRepository)
        {
            this._artifactsStorage = mavenTreeRepository;
            this._repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._artifactsRepository = mavenArtifactsRepository;
            _versionedArtifactsRepository = mavenSearchRepository;
            this._versionedClassifiersRepository = versionedClassifiersRepository;
        }

        public ExploreResult Explore(Guid repoId, MavenIndex explore)
        {
            var repo = _repositoryEntitiesRepository.GetById(repoId);
            var result = new ExploreResult();
            var baseUrl = "/" + repo.Prefix;
            result.Children = _artifactsStorage.GetSubDir(repo, explore);

            if (explore.Group != null && explore.Group.Any())
            {
                baseUrl += "/" + string.Join("/", explore.Group);
                if (!string.IsNullOrWhiteSpace(explore.ArtifactId))
                {
                    baseUrl += "/" + explore.ArtifactId;
                    if (string.IsNullOrWhiteSpace(explore.Version))
                    {
                        var metaDb = _artifactsRepository.GetMetadata(repo.Id, explore.Group, explore.ArtifactId);
                        if (!string.IsNullOrWhiteSpace(explore.Meta))
                        {

                            if (!string.IsNullOrWhiteSpace(explore.Checksum))
                            {
                                baseUrl = null;
                                result.Content = GetChecksum(metaDb.Checksums, explore.Checksum);
                            }
                            else
                            {
                                throw new Exception("retrieve the meta");
                            }
                        }
                        else
                        {
                            result.Children.Add("maven-metadata.xml");
                            foreach (var item in GetChecksums(metaDb.Checksums))
                            {
                                result.Children.Add("maven-metadata.xml." + item);
                            }
                            foreach(var arti in _versionedArtifactsRepository.GetArtifactData(repoId, explore.Group, explore.ArtifactId))
                            {
                                var snapshot = arti.IsSnapshot ? "-SNAPSHOT" : "";
                                result.Children.Add(arti.Version + snapshot);
                            }
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(explore.Version))
                    {
                        baseUrl += "/" + explore.Version;
                        var snapshot = explore.IsSnapshot ? "-SNAPSHOT" : "";
                        var artifact = _versionedArtifactsRepository.GetArtifactData(repo.Id, explore.Group, explore.ArtifactId, explore.Version, explore.IsSnapshot);
                        if (!string.IsNullOrWhiteSpace(explore.Type) || !string.IsNullOrWhiteSpace(explore.Classifier))
                        {
                            if (string.IsNullOrWhiteSpace(explore.Classifier))
                            {
                                if (!string.IsNullOrWhiteSpace(explore.Checksum))
                                {
                                    baseUrl = null;
                                    result.Content = GetChecksum(artifact.Checksums, explore.Checksum);
                                }
                                else
                                {
                                    throw new Exception("retrieve the artifact");
                                }
                            }
                            else
                            {
                                var classif = _versionedClassifiersRepository.GetArtifactData(repoId, explore.Group, explore.ArtifactId, explore.Version, explore.IsSnapshot, explore.Classifier);
                                if (!string.IsNullOrWhiteSpace(explore.Checksum))
                                {
                                    baseUrl = null;
                                    result.Content = GetChecksum(classif.Checksums, explore.Checksum);
                                }
                                else
                                {
                                    throw new Exception("retrieve the artifactclassifier");
                                }
                            }
                        }
                        else
                        {
                            result.Children.Add(artifact.ArtifactId + "-" + artifact.Version + snapshot + "." + artifact.Packaging);
                            foreach (var checksum in GetChecksums(artifact.Checksums))
                            {
                                result.Children.Add(artifact.ArtifactId + "-" + artifact.Version + snapshot + "." + artifact.Packaging + "." + checksum);
                            }

                            foreach (var classif in _versionedClassifiersRepository.GetArtifactData(repoId, explore.Group, explore.ArtifactId, explore.Version, explore.IsSnapshot))
                            {
                                result.Children.Add(classif.ArtifactId + "-" + classif.Classifer + "-" + classif.Version + snapshot + "." + classif.Packaging);
                                foreach (var checksum in GetChecksums(classif.Checksums))
                                {
                                    result.Children.Add(classif.ArtifactId + "-" + classif.Classifer + "-" + classif.Version + snapshot + "." + classif.Packaging + "." + checksum);
                                }
                            }
                        }
                    }
                }
            }
            result.Base = baseUrl;
            return result;
        }

        private byte[] GetChecksum(string checksums, string type)
        {
            var css = checksums.Split('|');
            var cs = css.First(a => a.StartsWith(type + "$")).Substring(type.Length + 1);
            return Encoding.UTF8.GetBytes(cs);
        }

        private List<string> GetChecksums(string checksums)
        {
            var css = checksums.Split('|');
            return css.Select(a => a.Split('$')[0]).ToList();
        }
    }
}
