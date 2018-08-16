using Maven.Repositories;
using Maven.Services;
using MavenProtocol;
using MavenProtocol.Apis;
using MultiRepositories.Repositories;
using Newtonsoft.Json;
using SemVer;
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
    public class MavenArtifactsService : IMavenArtifactsService
    {
        private readonly IMavenArtifactsRepository _mavenArtifactsRepository;
        private readonly IArtifactsStorage _artifactsStorage;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IMavenSearchRepository _mavenSearchRepository;
        private readonly IMavenSearchLastRepository _mavenSearchLastRepository;

        public MavenArtifactsService(
                IMavenArtifactsRepository mavenArtifactsRepository, IArtifactsStorage artifactsStorage,
                IRepositoryEntitiesRepository repositoryEntitiesRepository,
                IMavenSearchRepository mavenSearchRepository,
                IMavenSearchLastRepository mavenSearchLastRepository)
        {
            this._mavenArtifactsRepository = mavenArtifactsRepository;
            this._artifactsStorage = artifactsStorage;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._mavenSearchRepository = mavenSearchRepository;
            this._mavenSearchLastRepository = mavenSearchLastRepository;
        }

        public string ReadChecksum(Guid repoId, MavenIndex item)
        {
            var artifact = _mavenArtifactsRepository.GetById(repoId,
                item.Group, item.ArtifactId, item.Version, item.Classifier);
            if (item.Checksum == "md5") return artifact.Md5;
            if (item.Checksum == "sha1") return artifact.Sha1;
            if (item.Checksum == "asc") return artifact.Asc;
            return null;
        }

        public byte[] ReadArtifact(Guid repoId, MavenIndex item)
        {
            var repo = _repositoryEntitiesRepository.GetById(repoId);

            if (item.Type == "maven-metadata")
            {
                string result = ReadPackageMavenMetadata(repo.Id,item);
                return Encoding.UTF8.GetBytes(result);
            }
            else
            {
                return _artifactsStorage.Load(repo, item.Group, item.ArtifactId, item.Version, item.Classifier, item.Type);
            }
        }

        private string ReadPackageMavenMetadata(Guid repoId,MavenIndex item)
        {
            var metadata = new MavenMetadataXml();
            MavenSearchEntity latestSnapshot = null;
            metadata.ArtifactId = item.ArtifactId;
            metadata.GroupId = string.Join(".", item.Group);
            var artifacts = _mavenSearchRepository.GetByArtifactId(repoId, item.ArtifactId, metadata.GroupId);
            if (artifacts.Any())
            {
                var latestSnapshotVersion = SemVersion.Parse("0");
                metadata.Versioning = new MavenVersioning();
                if (artifacts.Any(a => a.Version.ToUpperInvariant().EndsWith("-SNAPSHOT")))
                {


                    metadata.Versioning.SnapshotVersions = new MavenSnapshotVersions();
                    metadata.Versioning.SnapshotVersions.Version = new List<MavenSnapshotVersion>();
                    foreach (var snap in artifacts.Where(a => a.Version.ToUpperInvariant().EndsWith("-SNAPSHOT")))
                    {
                        var classifiers = snap.Classifiers.Trim('|').Split('|');
                        var types = snap.Type.Trim('|').Split('|');

                        for (var i = 0; i < classifiers.Length; i++)
                        {
                            var classifier = classifiers[i] == "null" ? null : classifiers[i];
                            var type = types[i] == "null" ? "jar" : types[i];
                            var snave = new MavenSnapshotVersion
                            {
                                Classifier = classifier,
                                Extension = type,
                                Updated = snap.Timestamp.ToFileTime().ToString(),
                                Value = snap.BuildNumer
                            };
                            var curVer = SemVersion.Parse(snap.Version);
                            if (curVer > latestSnapshotVersion)
                            {
                                latestSnapshotVersion = curVer;
                                latestSnapshot = snap;
                            }
                            metadata.Versioning.SnapshotVersions.Version.Add(snave);
                        }
                    }
                }
                if (artifacts.Any(a => !a.Version.ToUpperInvariant().EndsWith("-SNAPSHOT")))
                {
                    var maxVersionRelease = new SemVersion(0);

                    metadata.Versioning.Versions = new MavenVersions();
                    metadata.Versioning.Versions.Version = new List<string>();
                    foreach (var rel in artifacts.Where(a => !a.Version.ToUpperInvariant().EndsWith("-SNAPSHOT")))
                    {
                        metadata.Versioning.Versions.Version.Add(rel.Version);
                    }
                }
            }
            var latest = _mavenSearchLastRepository.GetByArtifactId(repoId,item.ArtifactId, metadata.GroupId);
            if (!string.IsNullOrWhiteSpace(latest.JsonPlugins))
            {
                metadata.Plugins = JsonConvert.DeserializeObject<MavenPlugins>(latest.JsonPlugins);
            }

            metadata.Versioning.LastUpdated = latest.Timestamp.ToFileTime().ToString();
            metadata.Versioning.Latest = latest.Version;
            if (!string.IsNullOrWhiteSpace(latest.VersionRelease))
            {
                metadata.Versioning.Release = latest.VersionRelease;
            }

            if (latestSnapshot != null)
            {
                metadata.Versioning.Snapshot = new MavenSnapshot
                {
                    BuildNumber = latestSnapshot.BuildNumer,
                    Timestamp = latestSnapshot.Timestamp.ToFileTime().ToString()
                };
            }
            XmlSerializer xsSubmit = new XmlSerializer(metadata.GetType());
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, metadata);
                    return sww.ToString(); // Your XML
                }
            }
        }

        public void WriteChecksum(Guid repoId, MavenIndex item, string content)
        {
            var artifact = _mavenArtifactsRepository.GetById(repoId,
                item.Group, item.ArtifactId, item.Version, item.Classifier);
            if (item.Checksum == "md5") artifact.Md5 = content;
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
