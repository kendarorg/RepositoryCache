using Maven.Repositories;
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
    public class MavenArtifactsService : IMavenArtifactsService
    {
        private readonly IMavenArtifactsRepository _mavenArtifactsRepository;
        private readonly IArtifactsStorage _artifactsStorage;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;

        public MavenArtifactsService(
            IMavenArtifactsRepository mavenArtifactsRepository, IArtifactsStorage artifactsStorage,
             IRepositoryEntitiesRepository repositoryEntitiesRepository)
        {
            this._mavenArtifactsRepository = mavenArtifactsRepository;
            this._artifactsStorage = artifactsStorage;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
        }
        public string ReadChecksum(Guid repoId, MavenIndex item)
        {
            var artifact = _mavenArtifactsRepository.GetById(repoId, 
                item.Group, item.ArtifactId, item.Version,item.Classifier);
            if (item.Checksum == "md5") return artifact.Md5;
            if (item.Checksum == "sha1") return artifact.Sha1;
            if (item.Checksum == "asc") return artifact.Asc;
            return null;
        }

        public byte[] ReadArtifact(Guid repoId, MavenIndex item)
        {
            var repo = _repositoryEntitiesRepository.GetById(repoId);
            return _artifactsStorage.Load(repo, item.Group, item.ArtifactId, item.Version, item.Classifier, item.Type);
        }
        
        public void WriteChecksum(Guid repoId, MavenIndex item, string content)
        {
            var artifact = _mavenArtifactsRepository.GetById(repoId,
                item.Group, item.ArtifactId, item.Version, item.Classifier);
            if (item.Checksum == "md5") artifact.Md5=content;
            if (item.Checksum == "sha1") artifact.Sha1 = content;
            if (item.Checksum == "asc") artifact.Asc = content;
            _mavenArtifactsRepository.Update(artifact);
        }

        public void WriteArtifact(Guid repoId, MavenIndex item, byte[] content)
        {
            throw new NotImplementedException();
        }

        public void DeleteArtifact(Guid repoId, MavenIndex item)
        {
            throw new NotImplementedException();
        }
    }
}
