﻿using Maven.News;
using MavenProtocol.Apis;
using MavenProtocol.News;
using Repositories;
using SemVer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MavenProtocol
{

    public class MetadataApi : IMetadataApi
    {
        private readonly IMetadataRepository _metadataRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly IServicesMapper _servicesMapper;
        private readonly IArtifactsRepository _artifactsRepository;
        private readonly IHashCalculator _hashCalculator;
        private readonly IReleaseArtifactRepository _artifactVersionsRepository;

        public MetadataApi( IReleaseArtifactRepository artifactVersionsRepository,
            IMetadataRepository metadataRepository, ITransactionManager transactionManager,
            IServicesMapper servicesMapper,
            IArtifactsRepository artifactsRepository,
            IHashCalculator hashCalculator)
        {
            this._metadataRepository = metadataRepository;
            this._transactionManager = transactionManager;
            this._servicesMapper = servicesMapper;
            this._artifactsRepository = artifactsRepository;
            this._hashCalculator = hashCalculator;
            this._artifactVersionsRepository = artifactVersionsRepository;
        }

        public MetadataApiResult Generate(MavenIndex mi)
        {
            MetadataEntity metadata = null;
            if (string.IsNullOrWhiteSpace(mi.Version))
            {
                if (_metadataRepository.GetArtifactMetadata(mi.RepoId, mi.Group, mi.ArtifactId) == null)
                {
                    metadata = GenerateArtifactMetadata(mi);
                }

            }
            else
            {
                if (_metadataRepository.GetArtifactMetadata(mi.RepoId, mi.Group, mi.ArtifactId) == null)
                {
                    GenerateArtifactMetadata(mi);
                }
                if (_metadataRepository.GetArtifactVersionMetadata(mi.RepoId, mi.Group, mi.ArtifactId, mi.Version, mi.IsSnapshot) == null)
                {
                    metadata = GenerateArtifactVersionMetadata(mi);
                }
            }
            return CreateResponse(metadata, !string.IsNullOrWhiteSpace(mi.Checksum));
        }

        private MetadataEntity GenerateArtifactVersionMetadata(MavenIndex mi)
        {
            using (var transaction = _transactionManager.BeginTransaction())
            {
                var metadata = new MetadataEntity
                {
                    ArtifactId = mi.ArtifactId,
                    Group = string.Join(".", mi.Group),
                    Version = mi.Version,
                    IsSnapshot= mi.IsSnapshot
                };

                var mavenMetadataXml = InitializeMavenMetadataXml(metadata, false);
                FillSingleVersionData(mi, mavenMetadataXml, transaction);
                SerializeMetadata(metadata, mavenMetadataXml, transaction);
                return metadata;
            }
        }

        private MetadataEntity GenerateArtifactMetadata(MavenIndex mi)
        {
            using (var transaction = _transactionManager.BeginTransaction())
            {
                var metadata = new MetadataEntity
                {
                    ArtifactId = mi.ArtifactId,
                    Group = string.Join(".", mi.Group)
                };

                MavenMetadataXml mavenMetadataXml = InitializeMavenMetadataXml(metadata, true);
                FillVersions(mi, mavenMetadataXml, transaction);
                SerializeMetadata(metadata, mavenMetadataXml, transaction);
                return metadata;
            }
        }

        public bool CanHandle(MavenIndex mi)
        {
            return !string.IsNullOrWhiteSpace(mi.Meta);
        }

        public MetadataApiResult Retrieve(MavenIndex mi)
        {
            if (string.IsNullOrWhiteSpace(mi.Version))
            {
                return HandleArtifactMetadata(mi);
            }
            else
            {
                return HandleArtifactVersionMetadata(mi);
            }
        }

        private MetadataApiResult HandleArtifactMetadata(MavenIndex mi)
        {
            var metadata = _metadataRepository.GetArtifactMetadata(mi.RepoId, mi.Group, mi.ArtifactId);
            if (metadata == null)
            {
                return null;
            }
            return CreateResponse(metadata, !string.IsNullOrWhiteSpace(mi.Checksum));
        }

        private static MetadataApiResult CreateResponse(MetadataEntity metadata, bool checksumsOnly)
        {
            MavenMetadataXml mavenMetadataXml = null;
            if (!checksumsOnly)
            {

                if (!string.IsNullOrWhiteSpace(metadata.Xml))
                {
                    var serializer = new XmlSerializer(typeof(MavenMetadataXml));
                    using (var reader = new StringReader(metadata.Xml))
                    {
                        mavenMetadataXml = (MavenMetadataXml)serializer.Deserialize(reader);
                        reader.Close();
                    }
                }
            }
            return new MetadataApiResult
            {
                Md5 = metadata.Md5,
                Sha1 = metadata.Sha1,
                Xml = mavenMetadataXml
            };
        }

        private void FillVersions(MavenIndex mi, MavenMetadataXml result, ITransaction transaction)
        {
            MetadataEntity latest = null;
            MetadataEntity release = null;
            JavaSemVersion releaseVersion = JavaSemVersion.Parse("0");
            foreach (var version in _metadataRepository.GetVersions(mi.RepoId, mi.Group, mi.ArtifactId, transaction))
            {

                result.Versioning.Versions.Version.Add(BuildFullVersion(version.Version, version.IsSnapshot));
                latest = FindLatestVersion(latest, version);
                if (!version.IsSnapshot)
                {
                    release = FindRelease(release, ref releaseVersion, version);
                }
            }
            if (latest != null)
            {
                result.Versioning.Latest = BuildFullVersion(latest.Version, latest.IsSnapshot);
            }
            if (release != null)
            {
                result.Versioning.Release = BuildFullVersion(release.Version, release.IsSnapshot);
            }
        }

        private string BuildFullVersion(object version, object isSnapshot)
        {
            throw new NotImplementedException();
        }

        private static MavenMetadataXml InitializeMavenMetadataXml(MetadataEntity metadata, bool artifactMetadata)
        {
            var result = new MavenMetadataXml
            {
                ArtifactId = metadata.ArtifactId,
                GroupId = metadata.Group,
                Versioning = new MavenVersioning()
            };
            if (artifactMetadata)
            {
                result.Versioning.Versions = new MavenVersions
                {
                    Version = new List<string>()
                };
            }
            return result;
        }

        private string BuildFullVersion(string version, bool isSnapshot)
        {
            return version + (isSnapshot ? "-SNAPSHOT" : "");
        }

        private static MetadataEntity FindRelease(MetadataEntity release, ref JavaSemVersion releaseVersion, MetadataEntity version)
        {
            if (release == null)
            {
                release = version;
                releaseVersion = JavaSemVersion.Parse(version.Version);
            }
            else
            {
                var possibleVersion = JavaSemVersion.Parse(version.Version);
                if (possibleVersion > releaseVersion)
                {
                    release = version;
                    releaseVersion = possibleVersion;
                }
            }
            return release;
        }

        private static MetadataEntity FindLatestVersion(MetadataEntity latest, MetadataEntity version)
        {
            if (latest == null)
            {
                latest = version;
            }
            else
            {
                if (latest.Timestamp < version.Timestamp)
                {
                    latest = version;
                }
            }

            return latest;
        }

        private MetadataApiResult HandleArtifactVersionMetadata(MavenIndex mi)
        {
            var metadata = _metadataRepository.GetArtifactVersionMetadata(mi.RepoId, mi.Group, mi.ArtifactId, mi.Version, mi.IsSnapshot);
            if (metadata == null)
            {
                return null;
            }

            return CreateResponse(metadata, !string.IsNullOrWhiteSpace(mi.Checksum));
        }

        private void FillSingleVersionData(MavenIndex mi, MavenMetadataXml mavenMetadataXml, ITransaction transaction)
        {
            var version = _artifactVersionsRepository.GetSingleVersion(mi.RepoId, mi.Group, mi.ArtifactId, mi.Version, mi.IsSnapshot, transaction);
            var hasTimestampedSnapshot = _servicesMapper.HasTimestampedSnapshot(mi.RepoId);


            mavenMetadataXml.Version = this.BuildFullVersion(version.Version, version.IsSnapshot);
            if (hasTimestampedSnapshot && version.IsSnapshot)
            {
                var fullBuildId = version.Timestamp.ToString("yyyyMMdd.HHmmss") + "-" + version.Build;
                mavenMetadataXml.Versioning.Snapshot = new MavenSnapshot
                {
                    Timestamp = version.Timestamp.ToString("yyyyMMdd.HHmmss"),
                    BuildNumber = version.Build
                };
                mavenMetadataXml.Versioning.LastUpdated = version.Timestamp.ToString("yyyyMMddHHmmss");
                mavenMetadataXml.Versioning.SnapshotVersions = new MavenSnapshotVersions
                {
                    Version = new List<MavenSnapshotVersion>()
                };
                foreach (var artifact in _artifactsRepository.GetSnapshotBuildArtifacts(mi.RepoId, mi.Group, mi.ArtifactId, mi.Version, version.Timestamp, version.Build))
                {
                    mavenMetadataXml.Versioning.SnapshotVersions.Version.Add(new MavenSnapshotVersion
                    {
                        Classifier = artifact.Classifier,
                        Extension = artifact.Extension,
                        Value = artifact.Version + "-" + artifact.Timestamp.ToString("yyyyMMdd.HHmmss") + "-" + artifact.Build,
                        Updated = artifact.Timestamp.ToString("yyyyMMddHHmmss")
                    });
                }
            }
        }

        private void SerializeMetadata(MetadataEntity metadata, MavenMetadataXml mavenMetadataXml, ITransaction transaction)
        {
            var xsSubmit = new XmlSerializer(typeof(MavenMetadataXml));

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
            _metadataRepository.Save(metadata, transaction);
        }
    }
}