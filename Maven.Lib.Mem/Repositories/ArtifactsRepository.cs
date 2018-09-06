using Maven.News;
using MultiRepositories;
using Repositories;
using System;
using System.Collections.Generic;

namespace Maven.Repositories
{
    public class ArtifactsRepository : InMemoryRepository<ArtifactEntity>, IArtifactsRepository
    {
        public ArtifactsRepository(AppProperties properties) : base(properties)
        {
        }

        public IEnumerable<ArtifactEntity> GetAllArtifacts(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public ArtifactEntity GetSingleArtifact(Guid repoId, string[] group, string artifactId, string version, string classifier, string extension, bool isSnapshot, DateTime timestamp, string build)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ArtifactEntity> GetSnapshotBuildArtifacts(Guid repoId, string[] group, string artifactId, string version, DateTime timestampToSeconds, string buildId, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }
    }
}
