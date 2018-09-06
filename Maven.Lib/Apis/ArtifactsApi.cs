
using Maven.Services;
using MavenProtocol;
using MavenProtocol.Apis;
using MavenProtocol.News;
using MultiRepositories.Repositories;
using Repositories;
using System;

namespace Maven.News
{


    public class ArtifactsApi : IArtifactsApi
    {
        private readonly IArtifactsRepository _artifactVersionsRepository;
        private readonly IArtifactsStorage _artifactsStorage;
        private readonly IRepositoryEntitiesRepository _repositoriesRepository;
        private readonly IServicesMapper _servicesMapper;
        private readonly IHashCalculator _hashCalculator;
        private readonly ITransactionManager _transactionManager;
        private readonly IPomApi _pomApi;

        public ArtifactsApi(IArtifactsRepository artifactVersionsRepository, IArtifactsStorage artifactsStorage,
            IRepositoryEntitiesRepository repositoriesRepository,
            IServicesMapper servicesMapper,
            IHashCalculator hashCalculator, ITransactionManager transactionManager,
            IPomApi pomApi)
        {
            this._artifactVersionsRepository = artifactVersionsRepository;
            this._artifactsStorage = artifactsStorage;
            this._repositoriesRepository = repositoriesRepository;
            this._servicesMapper = servicesMapper;
            this._hashCalculator = hashCalculator;
            this._transactionManager = transactionManager;
            this._pomApi = pomApi;
        }
        public bool CanHandle(MavenIndex mi)
        {
            return !string.IsNullOrWhiteSpace(mi.Version) && !string.IsNullOrWhiteSpace(mi.Extension) && string.IsNullOrWhiteSpace(mi.Meta) && mi.Extension != "pom";
        }

        public ArtifactsApiResult Retrieve(MavenIndex mi)
        {
            var artifact = _artifactVersionsRepository.GetSingleArtifact(mi.RepoId, mi.Group, mi.ArtifactId, mi.Version, mi.Classifier, mi.Extension, mi.IsSnapshot, mi.Timestamp, mi.Build);
            if (artifact == null)
            {
                return null;
            }
            var repo = _repositoriesRepository.GetById(mi.RepoId);
            var result = new ArtifactsApiResult
            {
                Md5 = artifact.Md5,
                Sha1 = artifact.Sha1,
            };
            if (!string.IsNullOrWhiteSpace(mi.Checksum))
            {
                result.Content = _artifactsStorage.Load(repo, mi);
            }
            return result;
        }

        public ArtifactsApiResult Generate(MavenIndex mi)
        {
            var artifact = _artifactVersionsRepository.GetSingleArtifact(mi.RepoId, mi.Group, mi.ArtifactId, mi.Version, mi.Classifier, mi.Extension, mi.IsSnapshot, mi.Timestamp, mi.Build);
            var isNew = true;
            if (artifact != null)
            {
                isNew = false;
                if (mi.IsSnapshot)
                {
                    if (_servicesMapper.HasTimestampedSnapshot(mi.RepoId))
                    {
                        throw new Exception("Cannot duplicate timstamped snapshots");
                    }
                }
                else
                {
                    throw new Exception("Cannot duplicate release artifacts");
                }
            }
            artifact = new ArtifactEntity
            {
                Build = mi.Build,
                Timestamp = mi.Timestamp,
                IsSnapshot = mi.IsSnapshot,
                Version = mi.Version,
                ArtifactId = mi.ArtifactId,
                Group = string.Join(".", mi.Group),
                Md5 = _hashCalculator.GetMd5(mi.Content),
                Sha1 = _hashCalculator.GetSha1(mi.Content)
            };
            var repo = _repositoriesRepository.GetById(mi.RepoId);
            using (var transactin = _transactionManager.BeginTransaction())
            {
                _pomApi.Generate(mi);
                _artifactsStorage.Save(repo, mi, mi.Content);
                if (isNew)
                {
                    _artifactVersionsRepository.Save(artifact, transactin);
                }
                else
                {
                    _artifactVersionsRepository.Update(artifact, transactin);
                }
            }
            var result = new ArtifactsApiResult
            {
                Md5 = artifact.Md5,
                Sha1 = artifact.Sha1,
            };
            if (!string.IsNullOrWhiteSpace(mi.Checksum))
            {
                result.Content = mi.Content;
            }
            return result;
        }
    }
}
