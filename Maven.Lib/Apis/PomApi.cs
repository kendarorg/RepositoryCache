using MavenProtocol;
using MavenProtocol.Apis;
using MavenProtocol.News;
using Repositories;
using SemVer;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Maven.News
{
    public class PomApi : IPomApi
    {
        private readonly IPomRepository _pomRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly IHashCalculator _hashCalculator;
        private readonly IMetadataApi _metadataApi;
        private readonly IArtifactsRepository _artifactsRepository;
        private readonly IReleasePomRepository _releasePomRepository;

        public PomApi(IPomRepository pomRepository, ITransactionManager transactionManager,
            IHashCalculator hashCalculator,
            IMetadataApi metadataApi, IArtifactsRepository artifactsRepository,
            IReleasePomRepository releasePomRepository)
        {
            this._pomRepository = pomRepository;
            this._transactionManager = transactionManager;
            this._hashCalculator = hashCalculator;
            this._metadataApi = metadataApi;
            this._artifactsRepository = artifactsRepository;
            this._releasePomRepository = releasePomRepository;
        }
        public bool CanHandle(MavenIndex mi)
        {
            return mi.Extension == "pom";
        }

        public PomApiResult Retrieve(MavenIndex mi)
        {
            var metadata = _pomRepository.GetSinglePom(mi.RepoId, mi.Group, mi.ArtifactId, mi.Version, mi.IsSnapshot, mi.Timestamp, mi.Build);
            if (metadata == null)
            {
                return null;
            }

            return CreateResponse(metadata, !string.IsNullOrWhiteSpace(mi.Checksum));
        }

        private static PomApiResult CreateResponse(PomEntity metadata, bool checksumsOnly)
        {
            PomXml mavenMetadataXml = null;
            if (!checksumsOnly)
            {
                if (!string.IsNullOrWhiteSpace(metadata.Xml))
                {
                    var serializer = new XmlSerializer(typeof(PomXml));
                    using (var reader = new StringReader(metadata.Xml))
                    {
                        mavenMetadataXml = (PomXml)serializer.Deserialize(reader);
                        reader.Close();
                    }
                }
            }
            return new PomApiResult
            {
                Md5 = metadata.Md5,
                Sha1 = metadata.Sha1,
                Xml = checksumsOnly ? null : metadata.OriginalXml
            };
        }

        private void SerializePom(PomEntity pomEntity, PomXml mavenMetadataXml, ITransaction transaction)
        {
            SetupPomStringAndChecksum(pomEntity, mavenMetadataXml);

            _pomRepository.Save(pomEntity, transaction);

            var release = _releasePomRepository.GetSinglePom(pomEntity.RepositoryId,
                pomEntity.Group.Split('.'), pomEntity.ArtifactId, pomEntity.IsSnapshot, transaction);

            if (release == null)
            {
                release = SaveClonedPomEntity(pomEntity, transaction);
            }
            else
            {
                SavePomOnVersionChanged(pomEntity, transaction, release);
            }
        }

        private void SetupPomStringAndChecksum(PomEntity pomEntity, PomXml mavenMetadataXml)
        {
            var xsSubmit = new XmlSerializer(typeof(PomXml));

            using (var sww = new StringWriter())
            {
                using (var writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, mavenMetadataXml);
                    pomEntity.Xml = sww.ToString();
                }
            }
            pomEntity.Md5 = _hashCalculator.GetMd5(pomEntity.OriginalXml);
            pomEntity.Sha1 = _hashCalculator.GetSha1(pomEntity.OriginalXml);
        }

        private void SavePomOnVersionChanged(PomEntity pomEntity, ITransaction transaction, PomEntity release)
        {
            var oldp = SemVersion.Parse(pomEntity.Version);
            var relp = SemVersion.Parse(release.Version);
            if (oldp > relp)
            {
                if (pomEntity.IsSnapshot == release.IsSnapshot)
                {
                    pomEntity.Clone(release);
                    _releasePomRepository.Save(release, transaction);
                }
            }
        }

        private PomEntity SaveClonedPomEntity(PomEntity pomEntity, ITransaction transaction)
        {
            PomEntity release = new PomEntity();
            pomEntity.Clone(release);
            _releasePomRepository.Save(release, transaction);
            return release;
        }

        public PomApiResult Generate(MavenIndex mi, bool remote)
        {
            PomApiResult result = null;
            using (var transaction = _transactionManager.BeginTransaction())
            {
                var strPom = Encoding.UTF8.GetString(mi.Content);
                var metadata = _pomRepository.GetSinglePom(mi.RepoId,
                    mi.Group, mi.ArtifactId, mi.Version, mi.IsSnapshot, mi.Timestamp, mi.Build);

                if (metadata != null && remote)
                {
                    result = GenerateRemoteResult(mi, metadata);
                }
                else if (string.IsNullOrWhiteSpace(mi.Checksum))
                {
                    metadata = GenerateMetadata(mi, strPom, metadata);
                    InitializeClassifiersAndPackaging(mi, metadata);
                    SerializePom(metadata, PomXml.Parse(strPom), transaction);
                    result = CreateResponse(metadata, !string.IsNullOrWhiteSpace(mi.Checksum));
                }
                return result;
            }
        }

        private void InitializeClassifiersAndPackaging(MavenIndex mi, PomEntity metadata)
        {
            var classifiers = "|";
            var packaging = "|";
            foreach (var art in _artifactsRepository.GetSnapshotBuildArtifacts(mi.RepoId,
                mi.Group, mi.ArtifactId, mi.Version, mi.Timestamp, mi.Build))
            {
                packaging += art.Extension + "|";
                if (!string.IsNullOrWhiteSpace(art.Classifier))
                {
                    classifiers += art.Classifier + "|";
                }
                else
                {
                    classifiers += "%" + "|";
                }
            }
            metadata.Packaging = packaging;
            metadata.Classifiers = classifiers;
        }

        private static PomEntity GenerateMetadata(MavenIndex mi, string strPom, PomEntity metadata)
        {
            if (metadata == null)
            {
                metadata = new PomEntity
                {
                    RepositoryId = mi.RepoId,
                    ArtifactId = mi.ArtifactId,
                    Group = string.Join(".", mi.Group),
                    Version = mi.Version,
                    IsSnapshot = mi.IsSnapshot,
                    Build = mi.Build,
                    Timestamp = mi.Timestamp,
                    OriginalXml = strPom,
                };
            }

            return metadata;
        }

        private static PomApiResult GenerateRemoteResult(MavenIndex mi, PomEntity metadata)
        {
            PomApiResult result = new PomApiResult
            {
                Md5 = metadata.Md5,
                Sha1 = metadata.Sha1,
            };
            if (string.IsNullOrWhiteSpace(mi.Checksum))
            {
                result.Xml = metadata.OriginalXml;
            }

            return result;
        }

        public void UpdateClassifiers(MavenIndex mi)
        {
            var metadata = _pomRepository.GetSinglePom(mi.RepoId,
                    mi.Group, mi.ArtifactId, mi.Version, mi.IsSnapshot, mi.Timestamp, mi.Build);
            if (metadata == null)
            {
                return;
            }
            var classifiers = "|";
            var packaging = "|";
            foreach (var art in _artifactsRepository.GetSnapshotBuildArtifacts(mi.RepoId,
                mi.Group, mi.ArtifactId, mi.Version, mi.Timestamp, mi.Build))
            {
                packaging += art.Extension + "|";
                if (!string.IsNullOrWhiteSpace(art.Classifier))
                {
                    classifiers += art.Classifier + "|";
                }
                else
                {
                    classifiers += "%" + "|";
                }
            }
            metadata.Packaging = packaging;
            metadata.Classifiers = classifiers;
            _pomRepository.Update(metadata);
        }
    }
}
