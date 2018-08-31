using Maven.Repositories;
using MavenProtocol.Apis;
using MultiRepositories.Repositories;
using Newtonsoft.Json;
using Maven.Services;
using Repositories;
using SemVer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

        private string RebuildChecksun(string oldChecksum, string newChecksumType, string stringChecksum)
        {
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

        public void SetArtifactChecksums(Guid repoId, MavenIndex idx, string checksum)
        {
            using (var transaction = _transactionManager.BeginTransaction())
            {
                if (string.IsNullOrWhiteSpace(idx.Classifier))
                {
                    var artifactData = _versionedArtifactRepository.GetArtifactData(repoId, idx.Group, idx.ArtifactId, idx.Version, idx.IsSnapshot,transaction);
                    if (idx.Type == "pom")
                    {
                        artifactData.PomChecksums = RebuildChecksun(artifactData.PomChecksums, idx.Checksum, checksum);
                    }
                    else
                    {
                        artifactData.Checksums = RebuildChecksun(artifactData.Checksums, idx.Checksum, checksum);
                    }
                    _versionedArtifactRepository.Update(artifactData, transaction);
                }
                else
                {
                    var artifactData = _versionedClassifiersRepository.GetArtifactData(repoId, idx.Group, idx.ArtifactId, idx.Version, idx.IsSnapshot, idx.Classifier,transaction);
                    artifactData.Checksums = RebuildChecksun(artifactData.Checksums, idx.Checksum, checksum);
                    _versionedClassifiersRepository.Update(artifactData,transaction);
                }
                transaction.Commit();
            }


        }

        public void SetMetadataChecksums(Guid repoId, MavenIndex idx, string stringChecksum)
        {
            using (var transaction = _transactionManager.BeginTransaction())
            {
                var metadata = _artifactRepository.GetMetadata(repoId, idx.Group, idx.ArtifactId,transaction);
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


                var metadataDb = _artifactRepository.GetMetadata(repoId, idx.Group, idx.ArtifactId,transaction);
                bool isNew = SetupMetadata(idx, ref metadataDb);

                if (!string.IsNullOrWhiteSpace(content))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(MavenMetadataXml));
                    using (StringReader stringreader = new StringReader(content))
                    {
                        metadata = (MavenMetadataXml)xmlSerializer.Deserialize(stringreader);
                    }
                    metadataDb.Xml = content;
                }

                SaveMetadata(metadataDb, isNew,transaction);
                transaction.Commit();
            }
        }

        private static bool SetupMetadata(MavenIndex idx, ref ArtifactEntity metadataDb)
        {
            var isNew = false;
            if (metadataDb == null)
            {
                metadataDb = new ArtifactEntity();
                isNew = true;
            }
            metadataDb.ArtifactId = idx.ArtifactId;
            metadataDb.Group = string.Join(".", idx.Group);
            return isNew;
        }

        public void UploadArtifact(Guid repoId, MavenIndex idx, byte[] content)
        {
            using (var transaction = _transactionManager.BeginTransaction())
            {
                var artifactData = _versionedArtifactRepository.GetArtifactData(repoId, idx.Group, idx.ArtifactId, idx.Version, idx.IsSnapshot, transaction);
                var metadataDb = _artifactRepository.GetMetadata(repoId, idx.Group, idx.ArtifactId,transaction);
                bool isNewMetadata = SetupMetadata(idx, ref metadataDb);

                bool isNewArtifactData = BuildArtifactData(idx, ref artifactData, metadataDb);

                if (string.IsNullOrWhiteSpace(idx.Classifier))
                {
                    BuildArtifactSpecific(repoId, idx, content, artifactData,transaction);
                    BuildInferredMetadata(idx, artifactData, metadataDb, isNewMetadata, transaction);
                }

                SaveArtifactData(artifactData, isNewArtifactData, transaction);

                if (!string.IsNullOrWhiteSpace(idx.Classifier))
                {
                    BuildClassifier(repoId, idx, artifactData, metadataDb, content, transaction);
                }
                if (metadataDb.Release == artifactData.Version && !idx.IsSnapshot)
                {
                    BuildRelease(repoId, idx, artifactData);

                }
                transaction.Commit();
            }
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
                SaveRelease(isReleaseNew, release,transaction);
                transaction.Commit();
            }
        }

        private void SaveRelease(bool isReleaseNew, ReleaseEntity release,ITransaction transaction)
        {
            if (isReleaseNew)
            {
                _releaseArtifacts.Save(release, transaction);
            }
            else
            {
                _releaseArtifacts.Update(release, transaction);
            }
        }

        private static bool BuildArtifactData(MavenIndex idx, ref VersionedArtifactEntity artifactData, ArtifactEntity metadataDb)
        {
            var isNewArtifactData = false;
            if (artifactData == null)
            {
                isNewArtifactData = true;
                artifactData = new VersionedArtifactEntity
                {
                    Classifiers = string.Empty
                };
            }
            artifactData.OwnerMetadataId = metadataDb.Id;
            artifactData.BuildNumber = Guid.NewGuid().ToString();
            artifactData.Timestamp = DateTime.Now;
            artifactData.ArtifactId = idx.ArtifactId;
            artifactData.IsSnapshot = idx.IsSnapshot;
            artifactData.Version = idx.Version;
            artifactData.Group = string.Join(".", idx.Group);

            return isNewArtifactData;
        }

        private void BuildArtifactSpecific(Guid repoId, MavenIndex idx, byte[] content, VersionedArtifactEntity artifactData,ITransaction tra)
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
                SaveArtifactPackage(repoId, idx, content);
            }
        }

        private void BuildInferredMetadata(MavenIndex idx, VersionedArtifactEntity artifactData, ArtifactEntity metadataDb, bool isNewMetadata,ITransaction transaction)
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
            SaveMetadata(metadataDb, isNewMetadata, transaction);
        }

        private void BuildClassifier(Guid repoId, MavenIndex idx, VersionedArtifactEntity artifactData, ArtifactEntity metadataDb, byte[] content,ITransaction transaction)
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
            artifactDataClassifier.Classifer = idx.Classifier;

            SaveClassifierData(isNewArtifactDataClassifier, artifactDataClassifier,transaction);

            var classifiers = artifactData.Classifiers.Trim('|').Split('|');
            if (!classifiers.Any(a => a == idx.Classifier))
            {
                artifactData.Classifiers = string.Format("|{0}|{1}|", string.Join("|", classifiers), idx.Classifier);
                _versionedArtifactRepository.Update(artifactData);
            }
            SaveArtifactPackage(repoId, idx, content);
        }

        private void SaveMetadata(ArtifactEntity metadataDb, bool isNew,ITransaction transaction)
        {
            if (isNew)
            {
                _artifactRepository.Save(metadataDb, transaction);
            }
            else
            {
                _artifactRepository.Update(metadataDb, transaction);
            }
        }

        private void SaveClassifierData(bool isNewArtifactDataClassifier, VersionedClassifierEntity artifactDataClassifier, ITransaction transaction)
        {
            if (isNewArtifactDataClassifier)
            {
                _versionedClassifiersRepository.Save(artifactDataClassifier, transaction);
            }
            else
            {
                _versionedClassifiersRepository.Update(artifactDataClassifier, transaction);
            }
        }

        private void SaveArtifactData(VersionedArtifactEntity artifactData, bool isNewArtifactData, ITransaction transaction)
        {
            if (isNewArtifactData)
            {
                _versionedArtifactRepository.Save(artifactData, transaction);
            }
            else
            {
                _versionedArtifactRepository.Update(artifactData, transaction);
            }
        }

        private void SaveArtifactPackage(Guid repoId, MavenIndex idx, byte[] content)
        {
            var repo = _repository.GetById(repoId);
            _artifactsStorage.Save(repo, idx, content);
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
    }
}
