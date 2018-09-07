using MavenProtocol;
using MavenProtocol.Apis;
using MavenProtocol.News;
using Repositories;
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

        public PomApi(IPomRepository pomRepository, ITransactionManager transactionManager,
            IHashCalculator hashCalculator,
            IMetadataApi metadataApi, IArtifactsRepository artifactsRepository)
        {
            this._pomRepository = pomRepository;
            this._transactionManager = transactionManager;
            this._hashCalculator = hashCalculator;
            this._metadataApi = metadataApi;
            this._artifactsRepository = artifactsRepository;
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
                Xml = checksumsOnly?null:metadata.OriginalXml
            };
        }

        private void SerializePom(PomEntity metadata, PomXml mavenMetadataXml, ITransaction transaction)
        {
            var xsSubmit = new XmlSerializer(typeof(PomXml));

            using (var sww = new StringWriter())
            {
                using (var writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, mavenMetadataXml);
                    metadata.Xml = sww.ToString();
                }
            }
            metadata.Md5 = _hashCalculator.GetMd5(metadata.OriginalXml);
            metadata.Sha1 = _hashCalculator.GetSha1(metadata.OriginalXml);
            _pomRepository.Save(metadata, transaction);
        }

        public PomApiResult Generate(MavenIndex mi,bool remote)
        {
            
            using (var transaction = _transactionManager.BeginTransaction())
            {
                var strPom = Encoding.UTF8.GetString(mi.Content);
                var metadata = _pomRepository.GetSinglePom(mi.RepoId,
                    mi.Group, mi.ArtifactId, mi.Version, mi.IsSnapshot, mi.Timestamp, mi.Build);
                if (!string.IsNullOrWhiteSpace(mi.Checksum))
                {
                    return null;
                }
                if (metadata == null)
                {
                    
                    metadata = new PomEntity
                    {
                        FreeText = string.Empty,
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
                }
                metadata.Packaging = packaging;
                metadata.Classifiers = classifiers;

                if (!remote)
                {
                    var pom = PomXml.Parse(strPom);
                    SerializePom(metadata, pom, transaction);
                
                    var result = CreateResponse(metadata, !string.IsNullOrWhiteSpace(mi.Checksum));
                    //_metadataApi.Generate(mi,remote);
                    return result;
                }
                else
                {
                    var result = CreateResponse(metadata, !string.IsNullOrWhiteSpace(mi.Checksum));
                    _metadataApi.Generate(mi,remote);
                    return result;
                }
                
            }
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
            }
            metadata.Packaging = packaging;
            metadata.Classifiers = classifiers;
            _pomRepository.Update(metadata);
        }
    }
}
