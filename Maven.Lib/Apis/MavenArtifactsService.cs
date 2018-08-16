using Ioc;
using Ionic.Zip;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Maven.Apis
{
    public class MavenArtifactsService : IMavenArtifactsService, ISingleton
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
                string result = ReadPackageMavenMetadata(repo.Id, item);
                return Encoding.UTF8.GetBytes(result);
            }
            else
            {
                return _artifactsStorage.Load(repo, item.Group, item.ArtifactId, item.Version, item.Classifier, item.Type);
            }
        }



        private string ReadPackageMavenMetadata(Guid repoId, MavenIndex item)
        {
            var metadata = new MavenMetadataXml();
            MavenSearchEntity latestSnapshot = null;
            metadata.ArtifactId = item.ArtifactId;
            metadata.GroupId = string.Join(".", item.Group);
            var artifacts = _mavenSearchRepository.GetByArtifactId(repoId, item.ArtifactId, metadata.GroupId);
            if (artifacts.Any())
            {
                metadata.Versioning = new MavenVersioning();
                latestSnapshot = PrepareSnapshotData(metadata, artifacts);
                PrepareStandardData(metadata, artifacts);
                MavenSearchLastEntity latest = PreparePlugins(repoId, item, metadata);
                PrepareCurrentItems(metadata, latestSnapshot, latest);
            }
            return WriteMetadataXml(metadata);
        }

        private static void PrepareStandardData(MavenMetadataXml metadata, IEnumerable<MavenSearchEntity> artifacts)
        {
            if (artifacts.Any(a => !a.Version.ToUpperInvariant().EndsWith("-SNAPSHOT")))
            {
                var maxVersionRelease = new JavaSemVersion(0);

                metadata.Versioning.Versions = new MavenVersions();
                metadata.Versioning.Versions.Version = new List<string>();
                foreach (var rel in artifacts.Where(a => !a.Version.ToUpperInvariant().EndsWith("-SNAPSHOT")))
                {
                    metadata.Versioning.Versions.Version.Add(rel.Version);
                }
            }
        }

        private static MavenSearchEntity PrepareSnapshotData(MavenMetadataXml metadata, IEnumerable<MavenSearchEntity> artifacts)
        {
            MavenSearchEntity latestSnapshot = null;
            var latestSnapshotVersion = JavaSemVersion.Parse("0");
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
                        var curVer = JavaSemVersion.Parse(snap.Version);
                        if (curVer > latestSnapshotVersion)
                        {
                            latestSnapshotVersion = curVer;
                            latestSnapshot = snap;
                        }
                        metadata.Versioning.SnapshotVersions.Version.Add(snave);
                    }
                }
            }
            return latestSnapshot;
        }

        private MavenSearchLastEntity PreparePlugins(Guid repoId, MavenIndex item, MavenMetadataXml metadata)
        {
            var latest = _mavenSearchLastRepository.GetByArtifactId(repoId, item.ArtifactId, metadata.GroupId);
            if (!string.IsNullOrWhiteSpace(latest.JsonPlugins))
            {
                metadata.Plugins = JsonConvert.DeserializeObject<MavenPlugins>(latest.JsonPlugins);
            }

            return latest;
        }

        private static string WriteMetadataXml(MavenMetadataXml metadata)
        {
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

        private static void PrepareCurrentItems(MavenMetadataXml metadata, MavenSearchEntity latestSnapshot, MavenSearchLastEntity latest)
        {
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
            var type = item.Type.ToLowerInvariant();
            switch (type)
            {
                case ("pom"):
                    InsertPom(repoId, item, content);
                    break;
                case ("jar"):
                    InsertJar(repoId, item, content);
                    break;
                default:
                    Console.WriteLine("BOH " + type);
                    throw new NotImplementedException("NOT SUPPORTED " + type);
            }
        }

        private void InsertPom(Guid repoId, MavenIndex item, byte[] content)
        {
            var repo = _repositoryEntitiesRepository.GetById(repoId);
            PomXml result = null;
            var xml = Encoding.UTF8.GetString(content);
            string pattern = @"xmlns=""[a-zA-Z0-9:\/._]{1,}""";
            System.Text.RegularExpressions.Match m = Regex.Match(xml, pattern);
            if (m.Success)
            {
                xml = xml.Replace(m.Value, "");
            }
            result = PomXml.Parse(xml);

            _artifactsStorage.Write(repo, item.Group, item.ArtifactId, item.Version, item.Classifier, item.Type, content);
            if (string.IsNullOrWhiteSpace(item.Classifier))
            {
                InsertPom(result);
            }
        }

        private void InsertJar(Guid repoId, MavenIndex item, byte[] data)
        {
            var repo = _repositoryEntitiesRepository.GetById(repoId);
            _artifactsStorage.Write(repo, item.Group, item.ArtifactId, item.Version, item.Classifier, item.Type, data);
            if (string.IsNullOrWhiteSpace(item.Classifier))
            {
                PomXml result = null;
                byte[] pomContent = null;
                DateTime timestamp = DateTime.UtcNow;
                using (ZipFile zip = ZipFile.Read(new MemoryStream(data)))
                {
                    foreach (var entry in zip)
                    {
                        if (entry.FileName.ToLowerInvariant().EndsWith("pom.xml"))
                        {
                            timestamp = entry.ModifiedTime > entry.LastModified ? entry.ModifiedTime : entry.LastModified;
                            var ms = new MemoryStream();
                            entry.Extract(ms);
                            ms.Seek(0, SeekOrigin.Begin);
                            var ss = ms.ToArray();
                            var fileName = entry.FileName;
                            pomContent = ss;
                            var xml = Encoding.UTF8.GetString(ss);
                            string pattern = @"xmlns=""[a-zA-Z0-9:\/._]{1,}""";
                            System.Text.RegularExpressions.Match m = Regex.Match(xml, pattern);
                            if (m.Success)
                            {
                                xml = xml.Replace(m.Value, "");
                            }
                            result = PomXml.Parse(xml);
                            break;
                        }
                    }
                }
                if (result != null)
                {
                    _artifactsStorage.Write(repo, item.Group, item.ArtifactId, item.Version, item.Classifier, "pom", pomContent);
                    InsertPom(result);
                }
            }


        }

        private void InsertPom(PomXml result)
        {
            throw new NotImplementedException();
        }

        public void DeleteArtifact(Guid repoId, MavenIndex item)
        {
            throw new NotImplementedException();
        }
    }
}
