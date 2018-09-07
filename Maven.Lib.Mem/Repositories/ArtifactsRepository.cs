using Maven.News;
using MultiRepositories;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maven.Repositories
{
    public class ArtifactsRepository : InMemoryRepository<ArtifactEntity>, IArtifactsRepository
    {
        public ArtifactsRepository(AppProperties properties) : base(properties)
        {
        }

        public IEnumerable<ArtifactEntity> GetAllArtifacts(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, ITransaction transaction = null)
        {
            return GetAll().Where(a =>
            {
                return a.Group == string.Join(".", group) && a.ArtifactId == artifactId && a.Version == version && a.IsSnapshot == isSnapshot;
            });
        }

        public ArtifactEntity GetSingleArtifact(Guid repoId, string[] group, string artifactId, string version, string classifier, string extension, bool isSnapshot, DateTime timestamp, string build)
        {
            return GetAll().FirstOrDefault(a =>
            {
                var checkTimestamp = !string.IsNullOrWhiteSpace(a.Build) && timestamp.Year>1? a.Timestamp.ToFileTime() == timestamp.ToFileTime() : true;
                return a.Group == string.Join(".", group) && a.ArtifactId == artifactId && a.Version == version && a.IsSnapshot == isSnapshot &&
                    a.Classifier == classifier && a.Extension == extension && a.Build == build && checkTimestamp;
            });
        }

        public IEnumerable<ArtifactEntity> GetSnapshotBuildArtifacts(Guid repoId, string[] group, string artifactId, string version, DateTime timestamp, string build, ITransaction transaction = null)
        {
            return GetAll().Where(a =>
            {
                var isSnapshot = !string.IsNullOrWhiteSpace(build);
                var checkTimestamp = !string.IsNullOrWhiteSpace(a.Build) && timestamp.Year > 1 ? a.Timestamp.ToFileTime() == timestamp.ToFileTime() : true;
                return a.Group == string.Join(".", group) && a.ArtifactId == artifactId && a.Version == version && isSnapshot == a.IsSnapshot &&
                     a.Build == build && checkTimestamp;
            });
        }
    }
}
