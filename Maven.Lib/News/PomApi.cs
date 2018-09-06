﻿using MavenProtocol;
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

        public PomApi(IPomRepository pomRepository, ITransactionManager transactionManager,
            IHashCalculator hashCalculator,
            IMetadataApi metadataApi)
        {
            this._pomRepository = pomRepository;
            this._transactionManager = transactionManager;
            this._hashCalculator = hashCalculator;
            this._metadataApi = metadataApi;
        }
        public bool CanHandle(MavenIndex mi)
        {
            return mi.Extension == "pom";
        }

        public PomApiResult Retrieve(MavenIndex mi)
        {
            var metadata = _pomRepository.GetSinglePom(mi.RepoId, mi.Group, mi.ArtifactId, mi.Version, mi.Classifier, mi.Extension, mi.IsSnapshot, mi.Timestamp, mi.Build);
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
                Xml = mavenMetadataXml
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
            metadata.Md5 = _hashCalculator.GetMd5(metadata.Xml);
            metadata.Sha1 = _hashCalculator.GetSha1(metadata.Xml);
            _pomRepository.Save(metadata, transaction);
        }

        public PomApiResult Generate(MavenIndex mi)
        {
            using (var transaction = _transactionManager.BeginTransaction())
            {
                _metadataApi.Generate(mi);
                var strPom = Encoding.UTF8.GetString(mi.Content);
                var metadata = new PomEntity
                {
                    ArtifactId = mi.ArtifactId,
                    Group = string.Join(".", mi.Group),
                    Version = mi.Version,
                    IsSnapshot = mi.IsSnapshot,
                    Build = mi.Build,
                    Timestamp = mi.Timestamp,
                    OriginalXml = strPom
                };


                var pom = PomXml.Parse(strPom);
                SerializePom(metadata, pom, transaction);
                return CreateResponse(metadata, !string.IsNullOrWhiteSpace(mi.Checksum));
            }
        }
    }
}
