using Maven.Repositories;
using MavenProtocol.Apis;
using MultiRepositories.Repositories;
using Maven.Services;
using Repositories;
using SemVer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Maven;

namespace MavenProtocol.Test
{
    public class MavenArtifactsService : IMavenArtifactsService
    {
        private IArtifactRepository _artifactRepository;
        private IVersionedArtifactRepository _versionedArtifactRepository;
        private IVersionedClassifiersRepository _versionedClassifiersRepository;
        private IArtifactsStorage _artifactsStorage;
        private IReleaseArtifacts _releaseArtifacts;
        private readonly ITransactionManager _transactionManager;
        private IRepositoryEntitiesRepository _repository;

        public MavenArtifactsService(
            IRepositoryEntitiesRepository repository,
            IArtifactRepository artifactRepository,
            IVersionedArtifactRepository versionedArtifactRepository,
            IVersionedClassifiersRepository versionedClassifiersRepository,
            IArtifactsStorage artifactsStorage,
            IReleaseArtifacts releaseArtifacts,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _artifactRepository = artifactRepository;
            _versionedArtifactRepository = versionedArtifactRepository;
            this._versionedClassifiersRepository = versionedClassifiersRepository;
            this._artifactsStorage = artifactsStorage;
            this._releaseArtifacts = releaseArtifacts;
            this._transactionManager = transactionManager;
        }



        public void SetTags(Guid repoId, MavenIndex idx, string[] tags)
        {
            using (var transaction = _transactionManager.BeginTransaction())
            {
                if (idx.IsSnapshot)
                {
                    return;
                }
                var artifact = _versionedArtifactRepository.GetArtifactData(repoId, idx.Group, idx.ArtifactId, idx.Version, idx.IsSnapshot, transaction);
                artifact.Tags = "|" + string.Join("|", tags) + "|";
                _versionedArtifactRepository.Update(artifact);
                var release = _releaseArtifacts.GetByArtifact(repoId, idx.Group, idx.ArtifactId, transaction);
                if (release.Version == idx.Version)
                {
                    release.Tags = artifact.Tags;
                    _releaseArtifacts.Update(release);
                }
            }
        }

        public void SetArtifactChecksums(Guid repoId, MavenIndex idx, string checksum)
        {
            using (var transaction = _transactionManager.BeginTransaction())
            {
                var metadata = GenerateDummyMetadata(repoId, idx, transaction);
                var artifactData = GenerateDummyArtifact(repoId, idx, transaction, metadata);
                if (string.IsNullOrWhiteSpace(idx.Classifier))
                {
                    if (idx.Type == "pom")
                    {
                        artifactData.PomChecksums = RebuildChecksun(artifactData.PomChecksums, idx.Checksum, checksum);
                    }
                    else
                    {
                        artifactData.Packaging = idx.Type;
                        artifactData.Checksums = RebuildChecksun(artifactData.Checksums, idx.Checksum, checksum);
                    }
                    _versionedArtifactRepository.Update(artifactData, transaction);
                }
                else
                {
                    var classifierData = GenerateDummyClassifierArtifact(repoId, idx, transaction, artifactData);
                    classifierData.Checksums = RebuildChecksun(classifierData.Checksums, idx.Checksum, checksum);
                    classifierData.Packaging = idx.Type;
                    _versionedClassifiersRepository.Update(classifierData, transaction);
                }
                transaction.Commit();
            }


        }

        public void SetMetadataChecksums(Guid repoId, MavenIndex idx, string stringChecksum)
        {
            using (var transaction = _transactionManager.BeginTransaction())
            {
                var metadata = GenerateDummyMetadata(repoId, idx, transaction);
                metadata.Checksums = RebuildChecksun(metadata.Checksums, idx.Checksum, stringChecksum);
                _artifactRepository.Update(metadata, transaction);
                transaction.Commit();
            }
        }

        public void UpdateMetadata(Guid repoId, MavenIndex idx, string content)
        {
            using (var transaction = _transactionManager.BeginTransaction())
            {
                MavenMetadataXml metadata = null;
                var metadataDb = GenerateDummyMetadata(repoId, idx, transaction);

                if (!string.IsNullOrWhiteSpace(content))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(MavenMetadataXml));
                    using (StringReader stringreader = new StringReader(content))
                    {
                        metadata = (MavenMetadataXml)xmlSerializer.Deserialize(stringreader);
                    }
                    metadataDb.Xml = content;
                }

                _artifactRepository.Update(metadataDb, transaction);
                transaction.Commit();
            }
        }


        public void UploadArtifact(Guid repoId, MavenIndex idx, byte[] content)
        {
            using (var transaction = _transactionManager.BeginTransaction())
            {
                var metadataDb = GenerateDummyMetadata(repoId, idx, transaction);
                var artifactData = GenerateDummyArtifact(repoId, idx, transaction, metadataDb);

                if (string.IsNullOrWhiteSpace(idx.Classifier))
                {

                    if (idx.Type == "pom")
                    {
                        var pomString = Encoding.UTF8.GetString(content);
                        var pom = PomXml.Parse(pomString);
                        artifactData.Pom = pomString;

                    }
                    else
                    {
                        artifactData.Packaging = idx.Type;
                        var repo = _repository.GetById(repoId);
                        _artifactsStorage.Save(repo, idx, content);
                    }
                    BuildInferredMetadata(idx, artifactData, metadataDb, transaction);
                    if (metadataDb.Release == artifactData.Version && !idx.IsSnapshot)
                    {
                        BuildRelease(repoId, idx, artifactData);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(idx.Classifier))
                {
                    BuildClassifier(repoId, idx, artifactData, metadataDb, content, transaction);
                }
                _versionedArtifactRepository.Update(artifactData, transaction);
                transaction.Commit();
            }
        }


        private string RebuildChecksun(string oldChecksum, string newChecksumType, string stringChecksum)
        {
            if (oldChecksum == null)
            {
                oldChecksum = string.Empty;
            }
            var checksums = oldChecksum.Trim('|').Split('|');
            var newChecksums = new List<string>();
            var shouldAdd = true;
            foreach (var checksum in checksums)
            {
                if (checksum.StartsWith(newChecksumType + "$"))
                {
                    shouldAdd = false;
                    newChecksums.Add(newChecksumType + "$" + stringChecksum);
                }
                else
                {
                    newChecksums.Add(checksum);
                }
            }
            if (shouldAdd)
            {
                newChecksums.Add(newChecksumType + "$" + stringChecksum);
            }
            return "|" + string.Join("|", newChecksums) + "|";
        }

        private ArtifactEntity GenerateDummyMetadata(Guid repoId, MavenIndex idx, ITransaction transaction)
        {

            if (JavaSemVersion.TryParse(idx.ArtifactId, out JavaSemVersion test))
            {
                throw new InconsistentRemoteDataException();
            }
            var metadata = _artifactRepository.GetMetadata(repoId, idx.Group, idx.ArtifactId, transaction);
            if (metadata == null)
            {
                metadata = new ArtifactEntity
                {
                    ArtifactId = idx.ArtifactId,
                    Group = string.Join(".", idx.Group),
                    RepositoryId = repoId
                };
                _artifactRepository.Save(metadata, transaction);
            }
            return metadata;
        }


        private VersionedClassifierEntity GenerateDummyClassifierArtifact(Guid repoId, MavenIndex idx, ITransaction transaction, VersionedArtifactEntity artifactData)
        {
            var classifierData = _versionedClassifiersRepository.GetArtifactData(repoId, idx.Group, idx.ArtifactId, idx.Version, idx.IsSnapshot, idx.Classifier, transaction);
            if (classifierData == null)
            {
                classifierData.OwnerArtifactId = artifactData.Id;
                classifierData.ArtifactId = idx.ArtifactId;
                var ver = idx.Version + (idx.IsSnapshot ? "-SNAPSHOT" : "");
                classifierData.Version = ver;
                classifierData.IsSnapshot = idx.IsSnapshot;
                classifierData.Packaging = idx.Type;
            }
            return classifierData;
        }

        private VersionedArtifactEntity GenerateDummyArtifact(Guid repoId, MavenIndex idx, ITransaction transaction, ArtifactEntity metadata)
        {
            var artifactData = _versionedArtifactRepository.GetArtifactData(repoId, idx.Group, idx.ArtifactId, idx.Version, idx.IsSnapshot, transaction);
            if (artifactData == null)
            {
                var ver = idx.Version + (idx.IsSnapshot ? "-SNAPSHOT" : "");
                artifactData = new VersionedArtifactEntity
                {
                    ArtifactId = idx.ArtifactId,
                    OwnerMetadataId = metadata.Id,
                    Version = idx.Version,
                    IsSnapshot = idx.IsSnapshot,
                    BuildNumber = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.Now,
                    Group = string.Join(".", idx.Group),
                    RepositoryId = repoId
                };
                _versionedArtifactRepository.Save(artifactData, transaction);

            }

            return artifactData;
        }




        private void BuildRelease(Guid repoId, MavenIndex idx, VersionedArtifactEntity artifactData)
        {
            using (var transaction = _transactionManager.BeginTransaction())
            {
                var isReleaseNew = false;
                var release = _releaseArtifacts.GetByArtifact(repoId, idx.Group, idx.ArtifactId, transaction);
                if (release == null)
                {
                    isReleaseNew = true;
                    release = new ReleaseEntity();
                }
                release.Checksums = artifactData.Checksums;
                release.PomChecksums = artifactData.PomChecksums;
                release.ArtifactId = artifactData.ArtifactId;
                release.Version = artifactData.Version;
                release.Group = artifactData.Group;
                release.Pom = artifactData.Pom;
                release.Classifiers = artifactData.Classifiers;
                release.Packaging = artifactData.Packaging;
                release.Timestamp = artifactData.Timestamp;
                release.OwnerMetadataId = artifactData.OwnerMetadataId;
                release.BuildNumber = artifactData.BuildNumber;
                release.RepositoryId = repoId;
                if (isReleaseNew) _releaseArtifacts.Save(release, transaction); else _releaseArtifacts.Update(release, transaction);
                transaction.Commit();
            }
        }


        private void BuildInferredMetadata(MavenIndex idx, VersionedArtifactEntity artifactData, ArtifactEntity metadataDb, ITransaction transaction)
        {
            metadataDb.ArtifactId = idx.ArtifactId;
            metadataDb.Group = string.Join(".", idx.Group);
            metadataDb.Latest = idx.Version + (idx.IsSnapshot ? "-SNAPSHOT" : "");
            metadataDb.LastUpdated = artifactData.Timestamp;
            if (string.IsNullOrWhiteSpace(metadataDb.Release))
            {
                metadataDb.Release = "0.0.0";
            }
            if (string.IsNullOrWhiteSpace(metadataDb.Snapshot))
            {
                metadataDb.Snapshot = "0.0.0";
            }
            var prevSnapshot = SemVersion.Parse(metadataDb.Snapshot);
            var prevRelease = SemVersion.Parse(metadataDb.Release);
            var currRelease = SemVersion.Parse(idx.Version);

            if (!idx.IsSnapshot)
            {
                if (currRelease > prevRelease)
                {
                    metadataDb.Release = idx.Version;
                }
            }
            else
            {
                if (currRelease > prevSnapshot)
                {
                    metadataDb.Snapshot = idx.Version;
                }
            }
            _artifactRepository.Update(metadataDb);
        }

        private void BuildClassifier(Guid repoId, MavenIndex idx, VersionedArtifactEntity artifactData, ArtifactEntity metadataDb, byte[] content, ITransaction transaction)
        {
            var isNewArtifactDataClassifier = false;
            var artifactDataClassifier = _versionedClassifiersRepository.GetArtifactData(
                repoId, idx.Group, idx.ArtifactId, idx.Version, idx.IsSnapshot, idx.Classifier, transaction);
            if (artifactDataClassifier == null)
            {
                isNewArtifactDataClassifier = true;
                artifactDataClassifier = new VersionedClassifierEntity();
            }

            artifactDataClassifier.OwnerMetadataId = metadataDb.Id;
            artifactDataClassifier.Timestamp = DateTime.Now;
            artifactDataClassifier.Packaging = idx.Type;
            artifactDataClassifier.OwnerArtifactId = artifactData.Id;
            artifactDataClassifier.ArtifactId = idx.ArtifactId;
            artifactDataClassifier.IsSnapshot = idx.IsSnapshot;
            artifactDataClassifier.Version = idx.Version;
            artifactDataClassifier.Group = string.Join(".", idx.Group);
            artifactDataClassifier.Classifer = idx.Classifier ?? string.Empty;
            artifactDataClassifier.RepositoryId = repoId;
            if (isNewArtifactDataClassifier) _versionedClassifiersRepository.Save(artifactDataClassifier, transaction); else _versionedClassifiersRepository.Update(artifactDataClassifier, transaction);

            if (string.IsNullOrWhiteSpace(artifactData.Classifiers))
            {
                artifactData.Classifiers = "";
            }
            var classifiers = artifactData.Classifiers.Trim('|').Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (!classifiers.Any(a => a == idx.Classifier))
            {
                artifactData.Classifiers = string.Format("|{0}|{1}|", string.Join("|", classifiers), idx.Classifier);
                _versionedArtifactRepository.Update(artifactData);
            }
            var repo = _repository.GetById(repoId);
            _artifactsStorage.Save(repo, idx, content);
        }
    }
}
