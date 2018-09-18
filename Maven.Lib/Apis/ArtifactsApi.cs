
using Maven.Services;
using MavenProtocol;
using MavenProtocol.Apis;
using MavenProtocol.News;
using MultiRepositories.Repositories;
using Repositories;
using SemVer;
using System;

namespace Maven.News
{


    public class ArtifactsApi : IArtifactsApi
    {
        private readonly IArtifactsRepository _artifactsRepository;
        private readonly IArtifactsStorage _artifactsStorage;
        private readonly IRepositoryEntitiesRepository _repositoriesRepository;
        private readonly IServicesMapper _servicesMapper;
        private readonly IHashCalculator _hashCalculator;
        private readonly ITransactionManager _transactionManager;
        private readonly IReleaseArtifactRepository _releaseArtifactRepository;
        private readonly IPomApi _pomApi;
        private readonly IMetadataApi _metadataApi;

        public ArtifactsApi(IArtifactsRepository artifactVersionsRepository, IArtifactsStorage artifactsStorage,
            IRepositoryEntitiesRepository repositoriesRepository,
            IServicesMapper servicesMapper,
            IHashCalculator hashCalculator, ITransactionManager transactionManager,
            IReleaseArtifactRepository releaseArtifactRepository,
            IPomApi pomApi, IMetadataApi metadataApi)
        {
            this._artifactsRepository = artifactVersionsRepository;
            this._artifactsStorage = artifactsStorage;
            this._repositoriesRepository = repositoriesRepository;
            this._servicesMapper = servicesMapper;
            this._hashCalculator = hashCalculator;
            this._transactionManager = transactionManager;
            this._releaseArtifactRepository = releaseArtifactRepository;
            this._pomApi = pomApi;
            this._metadataApi = metadataApi;
        }
        public bool CanHandle(MavenIndex mi)
        {
            return !string.IsNullOrWhiteSpace(mi.Version) && !string.IsNullOrWhiteSpace(mi.Extension) && string.IsNullOrWhiteSpace(mi.Meta) && mi.Extension != "pom";
        }

        public ArtifactsApiResult Retrieve(MavenIndex mi)
        {
            var artifact = _artifactsRepository.GetSingleArtifact(mi.RepoId, mi.Group, mi.ArtifactId, mi.Version, mi.Classifier, mi.Extension, mi.IsSnapshot, mi.Timestamp, mi.Build);
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
            if (string.IsNullOrWhiteSpace(mi.Checksum))
            {
                result.Content = _artifactsStorage.Load(repo, mi);
            }
            return result;
        }

        public ArtifactsApiResult Generate(MavenIndex mi, bool remote)
        {
            var artifact = _artifactsRepository.GetSingleArtifact(mi.RepoId, mi.Group, mi.ArtifactId,
                mi.Version, mi.Classifier, mi.Extension, mi.IsSnapshot, mi.Timestamp, mi.Build);
            var repo = _repositoriesRepository.GetById(mi.RepoId);

            if (artifact != null && remote)
            {
                var remoteResult = new ArtifactsApiResult
                {
                    Md5 = artifact.Md5,
                    Sha1 = artifact.Sha1,
                };
                if (string.IsNullOrWhiteSpace(mi.Checksum))
                {
                    remoteResult.Content = mi.Content;
                    _artifactsStorage.Save(repo, mi, mi.Content);
                }
                return remoteResult;
            }
            if (artifact != null && !string.IsNullOrWhiteSpace(mi.Checksum))
            {
                return null;
            }

            if (artifact != null)
            {
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
                RepositoryId = mi.RepoId,
                Build = mi.Build,
                Timestamp = mi.Timestamp.Year > 1 ? mi.Timestamp : DateTime.Now,
                IsSnapshot = mi.IsSnapshot,
                Version = mi.Version,
                ArtifactId = mi.ArtifactId,
                Group = string.Join(".", mi.Group),
                Md5 = _hashCalculator.GetMd5(mi.Content),
                Sha1 = _hashCalculator.GetSha1(mi.Content),
                Extension = mi.Extension,
                Classifier = mi.Classifier
            };
            //var repo = _repositoriesRepository.GetById(mi.RepoId);
            using (var transactin = _transactionManager.BeginTransaction())
            {
                var release = _releaseArtifactRepository.GetForArtifact(
                    mi.RepoId, mi.Group, mi.ArtifactId, mi.IsSnapshot);
                if (release == null)
                {
                    release = new ReleaseVersion
                    {
                        ArtifactId = mi.ArtifactId,
                        Group = string.Join(".", mi.Group),
                        IsSnapshot = mi.IsSnapshot,
                        RepositoryId = mi.RepoId,
                        Version = "0",
                        Timestamp = mi.Timestamp.Year > 1 ? mi.Timestamp : DateTime.Now,
                    };
                }
                var prevVer = SemVersion.Parse(release.Version);
                var newVer = SemVersion.Parse(artifact.Version);
                if (newVer > prevVer)
                {
                    release.Timestamp = mi.Timestamp;
                    release.Build = mi.Build;
                    release.Version = mi.Version;
                }
                else if (newVer == prevVer && _servicesMapper.HasTimestampedSnapshot(mi.RepoId))
                {
                    release.Timestamp = mi.Timestamp;
                    release.Build = mi.Build;
                    release.Version = mi.Version;
                }
                _releaseArtifactRepository.Save(release, transactin);
                _artifactsStorage.Save(repo, mi, mi.Content);
                _artifactsRepository.Save(artifact, transactin);
                if (remote)
                {
                    _pomApi.Generate(mi, remote);
                }
                _pomApi.UpdateClassifiers(mi);
                if (!mi.IsSnapshot)
                {
                    _metadataApi.GenerateNoSnapshot(mi);
                }
            }
            var result = new ArtifactsApiResult
            {
                Md5 = artifact.Md5,
                Sha1 = artifact.Sha1,
            };
            if (string.IsNullOrWhiteSpace(mi.Checksum))
            {
                result.Content = mi.Content;
            }
            return result;
        }
    }
}
