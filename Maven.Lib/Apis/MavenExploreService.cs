using Maven.Repositories;
using Maven.Services;
using MavenProtocol;
using MavenProtocol.Apis;
using MavenProtocol.Apis.Browse;
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
    public class MavenExploreService : IMavenExploreService
    {
        private readonly IArtifactsStorage _artifactsStorage;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IArtifactRepository _artifactsRepository;
        private readonly IVersionedArtifactRepository _versionedArtifactsRepository;
        private readonly IVersionedClassifiersRepository _versionedClassifiersRepository;

        public MavenExploreService(IArtifactsStorage mavenTreeRepository,
            IRepositoryEntitiesRepository repositoryEntitiesRepository,
            IMavenArtifactsService mavenArtService,
            IArtifactRepository mavenArtifactsRepository,
            IVersionedArtifactRepository mavenSearchRepository,
            IVersionedClassifiersRepository versionedClassifiersRepository)
        {
            this._artifactsStorage = mavenTreeRepository;
            this._repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._artifactsRepository = mavenArtifactsRepository;
            _versionedArtifactsRepository = mavenSearchRepository;
            this._versionedClassifiersRepository = versionedClassifiersRepository;
        }

        public ExploreResult Explore(Guid repoId, MavenIndex explore)
        {
            var repo = _repositoryEntitiesRepository.GetById(repoId);
            var result = new ExploreResult();
            var baseUrl = "/" + repo.Prefix;
            result.Children = _artifactsStorage.GetSubDir(repo, explore);
            ArtifactEntity mainMetaData = FixPathWithArtifactId(explore, repo);

            if (explore.Group != null && explore.Group.Any())
            {
                baseUrl += "/" + string.Join("/", explore.Group);
                if (!string.IsNullOrWhiteSpace(explore.ArtifactId))
                {
                    baseUrl += "/" + explore.ArtifactId;
                    if (string.IsNullOrWhiteSpace(explore.Version))
                    {
                        if (!string.IsNullOrWhiteSpace(explore.Meta))
                        {
                            SetupArtifactDirectoryMetadataContent(repoId, explore, result, mainMetaData);
                        }
                        else
                        {
                            SetupArtifactDirectoryContent(repoId, explore, result, mainMetaData);
                        }
                    }
                    /*else if (!string.IsNullOrWhiteSpace(explore.Version) && !string.IsNullOrWhiteSpace(explore.Meta))
                    {
                        if (explore.IsSnapshot)
                        {

                            if (string.IsNullOrWhiteSpace(explore.Checksum))
                            {
                                result.Content = new byte[] { };
                            }
                            else
                            {
                                result.Content = new byte[] { };
                            }
                        }
                    }*/
                    else if (!string.IsNullOrWhiteSpace(explore.Version))
                    {
                        baseUrl += "/" + explore.Version;


                        if (!string.IsNullOrWhiteSpace(explore.Type) || !string.IsNullOrWhiteSpace(explore.Classifier))
                        {
                            var artifact = _versionedArtifactsRepository.GetSingleVersionedArtifact(repo.Id, explore.Group, explore.ArtifactId, explore.Version, explore.IsSnapshot, explore.Build);
                            if (string.IsNullOrWhiteSpace(explore.Classifier))
                            {
                                SetupMainArtifactContent(explore, repo, result, artifact);
                            }
                            else
                            {
                                SetupSubArtifactsContent(repoId, explore, repo, result);
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(explore.Meta))
                        {
                            var subMetaDb = _artifactsRepository.GetMetadata(repo.Id, explore.Group, explore.ArtifactId,
                                explore.Version, explore.IsSnapshot);
                            if (subMetaDb == null)
                            {
                                subMetaDb = new ArtifactEntity
                                {
                                    ArtifactId = explore.ArtifactId,
                                    Checksums = "",
                                    Group = string.Join(".", explore.Group),
                                    RepositoryId = repoId,
                                    Version = explore.Version,
                                    IsSnapshot = explore.IsSnapshot
                                };
                                _artifactsRepository.Save(subMetaDb);
                            }
                            if (string.IsNullOrWhiteSpace(subMetaDb.Checksums))
                            {
                                result.Content = new byte[] { };
                            }
                            else
                            {
                                if (subMetaDb.IsSnapshot) { }
                            }
                        }
                        else
                        {
                            //List specific artifact directory content
                            result.Children.Add("maven-metadata.xml");
                            foreach (var artifact in _versionedArtifactsRepository.GetAllMainArtifacts(repoId, explore.Group,
                                explore.ArtifactId, explore.Version, explore.IsSnapshot).ToList())
                            {
                                var snapshot = explore.IsSnapshot ? "-SNAPSHOT" : "";
                                if (artifact.IsSnapshot && !string.IsNullOrWhiteSpace(artifact.BuildNumber))
                                {
                                    snapshot = "-" + artifact.BuildNumber;
                                }
                                result.Children.Add(artifact.ArtifactId + "-" + artifact.Version + snapshot + "." + artifact.Packaging);
                                foreach (var checksum in GetChecksums(artifact.Checksums))
                                {
                                    result.Children.Add(artifact.ArtifactId + "-" + artifact.Version + snapshot + "." + artifact.Packaging + "." + checksum);
                                }

                                foreach (var classif in _versionedClassifiersRepository.GetAllClassifiersData(repoId, explore.Group,
                                    explore.ArtifactId, explore.Version, explore.IsSnapshot, artifact.BuildNumber))
                                {
                                    result.Children.Add(classif.ArtifactId + "-" + classif.Version + snapshot + "-" + classif.Classifer + "." + classif.Packaging);
                                    foreach (var checksum in GetChecksums(classif.Checksums))
                                    {
                                        result.Children.Add(classif.ArtifactId + "-" + classif.Version + snapshot + "-" + classif.Classifer + "." + classif.Packaging + "." + checksum);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            result.Base = baseUrl;
            return result;
        }

        private void SetupSubArtifactsContent(Guid repoId, MavenIndex explore, RepositoryEntity repo, ExploreResult result)
        {
            var classif = _versionedClassifiersRepository.GetSingleClassifierData(repoId, explore.Group, explore.ArtifactId, explore.Version, explore.IsSnapshot, explore.Classifier, explore.Build);
            if (!string.IsNullOrWhiteSpace(explore.Checksum))
            {
                result.Content = GetChecksum(classif.Checksums, explore.Checksum);
            }
            else
            {
                result.Content = _artifactsStorage.Load(repo, explore);
            }
        }

        private void SetupMainArtifactContent(MavenIndex explore, RepositoryEntity repo, ExploreResult result, VersionedArtifactEntity artifact)
        {
            if (!string.IsNullOrWhiteSpace(explore.Checksum))
            {
                result.Content = GetChecksum(artifact.Checksums, explore.Checksum);
            }
            else
            {
                result.Content = _artifactsStorage.Load(repo, explore);
            }
        }

        private void SetupArtifactDirectoryContent(Guid repoId, MavenIndex explore, ExploreResult result, ArtifactEntity mainMetaData)
        {
            result.Children.Add("maven-metadata.xml");
            foreach (var item in GetChecksums(mainMetaData.Checksums))
            {
                result.Children.Add("maven-metadata.xml." + item);
            }
            foreach (var arti in _versionedArtifactsRepository.GetAllMainArtifacts(repoId, explore.Group, explore.ArtifactId, explore.Version, explore.IsSnapshot))
            {
                var snapshot = arti.IsSnapshot ? "-SNAPSHOT" : "";
                result.Children.Add(arti.Version + snapshot);
            }
        }

        private void SetupArtifactDirectoryMetadataContent(Guid repoId, MavenIndex explore, ExploreResult result, ArtifactEntity mainMetaData)
        {
            if (!string.IsNullOrWhiteSpace(explore.Checksum))
            {
                result.Content = GetChecksum(mainMetaData.Checksums, explore.Checksum);
            }
            else
            {
                result.Content = BuildArtifactDirectoryMetadata(repoId, mainMetaData, explore);
            }
        }

        private ArtifactEntity FixPathWithArtifactId(MavenIndex explore, RepositoryEntity repo)
        {
            var subGroup = explore.Group.ToList().ToArray();
            if (subGroup.Length > 1)
            {
                subGroup = explore.Group.Take(explore.Group.Length - 1).ToArray();
            }
            ArtifactEntity metaDb = null;
            if (string.IsNullOrWhiteSpace(explore.ArtifactId) && explore.Group != null && explore.Group.Any())
            {
                metaDb = _artifactsRepository.GetMetadata(repo.Id, subGroup, explore.Group.Last());
                if (metaDb != null)
                {
                    var artifact = explore.Group.Last();
                    explore.Group = subGroup;
                    explore.ArtifactId = artifact;
                }
            }

            return metaDb;
        }


        private byte[] GetChecksum(string checksums, string type)
        {
            if (string.IsNullOrWhiteSpace(checksums))
            {
                throw new Exception();
            }
            var css = checksums.Split('|');
            var cs = css.First(a => a.StartsWith(type + "$")).Substring(type.Length + 1);
            return Encoding.UTF8.GetBytes(cs);
        }

        private List<string> GetChecksums(string checksums)
        {
            if (string.IsNullOrWhiteSpace(checksums))
            {
                return new List<string>();
            }
            var css = checksums.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return css.Select(a => a.Split('$')[0]).ToList();
        }

        private byte[] BuildArtifactDirectoryMetadata(Guid repoId, ArtifactEntity metaDb, MavenIndex item)
        {
            var metadata = new MavenMetadataXml();
            VersionedArtifactEntity latestSnapshot = null;
            VersionedArtifactEntity latestRelease = null;
            metadata.ArtifactId = item.ArtifactId;
            metadata.GroupId = string.Join(".", item.Group);
            var artifacts = _versionedArtifactsRepository.GetAllMainArtifacts(repoId, item.Group, item.ArtifactId, item.Version, item.IsSnapshot);
            if (artifacts.Any())
            {
                metadata.Versioning = new MavenVersioning();
                latestSnapshot = PrepareSnapshotDataAndAddToVersions(metadata, artifacts);
                latestRelease = PrepareStandardDataAndAddToVersions(metadata, artifacts);
                //PreparePlugins(repoId, item, metadata);
                PrepareCurrentItems(metadata, latestSnapshot, latestRelease);
            }
            if (string.IsNullOrWhiteSpace(metaDb.Checksums))
            {
                return new byte[] { };
            }
            return Encoding.UTF8.GetBytes(WriteMetadataXml(metadata));
        }

        private static VersionedArtifactEntity PrepareStandardDataAndAddToVersions(MavenMetadataXml metadata, IEnumerable<VersionedArtifactEntity> artifacts)
        {
            VersionedArtifactEntity result = null;
            if (artifacts.Any(a => !a.IsSnapshot))
            {
                var maxVersionRelease = new JavaSemVersion(0);

                metadata.Versioning.Versions = new MavenVersions
                {
                    Version = new List<string>()
                };
                foreach (var rel in artifacts.Where(a => !a.IsSnapshot))
                {
                    metadata.Versioning.Versions.Version.Add(rel.Version);
                    var curVersion = JavaSemVersion.Parse(rel.Version);
                    if (curVersion > maxVersionRelease)
                    {
                        maxVersionRelease = curVersion;
                        result = rel;
                    }
                }
            }
            return result;
        }

        private static VersionedArtifactEntity PrepareSnapshotDataAndAddToVersions(MavenMetadataXml metadata,
            IEnumerable<VersionedArtifactEntity> artifacts)
        {
            VersionedArtifactEntity latestSnapshot = null;
            var latestSnapshotVersion = JavaSemVersion.Parse("0");
            if (artifacts.Any(a => a.IsSnapshot))
            {
                /*metadata.Versioning.SnapshotVersions = new MavenSnapshotVersions
                {
                    Version = new List<MavenSnapshotVersion>()
                };*/
                foreach (var snap in artifacts.Where(a => a.IsSnapshot))
                {
                    //var classifiers = snap.Classifiers.Trim('|').Split('|');
                    //var types = snap.Type.Trim('|').Split('|');

                    //for (var i = 0; i < classifiers.Length; i++)
                    {
                        //Svar classifier = classifiers[i] == "null" ? null : classifiers[i];
                        /*var type = snap.Packaging;
                        var snave = new MavenSnapshotVersion
                        {
                            Classifier = null,
                            Extension = type,
                            Updated = snap.Timestamp.ToFileTime().ToString(),
                            Value = snap.BuildNumber
                        };*/
                        if (metadata.Versioning.Versions == null)
                        {
                            metadata.Versioning.Versions = new MavenVersions();
                        }
                        if (metadata.Versioning.Versions.Version == null)
                        {
                            metadata.Versioning.Versions.Version = new List<string>();
                        }
                        metadata.Versioning.Versions.Version.Add(snap.Version + "-SNAPSHOT");
                        var curVer = JavaSemVersion.Parse(snap.Version);
                        if (curVer > latestSnapshotVersion)
                        {
                            latestSnapshotVersion = curVer;
                            latestSnapshot = snap;
                        }
                        //metadata.Versioning.SnapshotVersions.Version.Add(snave);
                    }
                }
            }
            return latestSnapshot;
        }

        private void PreparePlugins(Guid repoId, MavenIndex item, MavenMetadataXml metadata)
        {
            var latest = _artifactsRepository.GetMetadata(repoId, item.Group, item.ArtifactId);
            if (!string.IsNullOrWhiteSpace(latest.JsonPlugins))
            {
                metadata.Plugins = JsonConvert.DeserializeObject<MavenPlugins>(latest.JsonPlugins);
            }
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

        private static void PrepareCurrentItems(
            MavenMetadataXml metadata,
            VersionedArtifactEntity latestSnapshot,
            VersionedArtifactEntity latest)
        {
            long lastUpdated = 0;
            var relSemVer = "0";
            if (latest != null)
            {
                lastUpdated = latest.Timestamp.ToFileTime();
                relSemVer = latest.Version;
                if (!string.IsNullOrWhiteSpace(latest.Version))
                {
                    metadata.Versioning.Release = latest.Version;
                }
            }

            if (latestSnapshot != null)
            {
                if (latestSnapshot.Timestamp.ToFileTime() > lastUpdated)
                {
                    lastUpdated = latestSnapshot.Timestamp.ToFileTime();
                    relSemVer = latestSnapshot.Version + "-SNAPSHOT";
                }
                metadata.Versioning.Snapshot = new MavenSnapshot
                {
                    BuildNumber = latestSnapshot.BuildNumber,
                    Timestamp = latestSnapshot.Timestamp.ToFileTime().ToString()
                };
            }

            metadata.Versioning.Latest = relSemVer;
            metadata.Versioning.LastUpdated = lastUpdated.ToString();
        }

    }
}
