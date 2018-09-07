using Maven.News;
using MultiRepositories;
using Repositories;
using System;
using System.Linq;

namespace Maven.Repositories
{
    public class ReleaseArtifactRepository : InMemoryRepository<ReleaseVersion>, IReleaseArtifactRepository
    {
        public ReleaseArtifactRepository(AppProperties properties) : base(properties)
        {
        }

        public ReleaseVersion GetForArtifact(Guid repoId, string[] group, string artifactId, bool isSnapshot)
        {
            return GetAll().FirstOrDefault(a =>
            {
                return a.RepositoryId == repoId && a.Group == string.Join(".", group) && 
                a.ArtifactId == artifactId && a.IsSnapshot == isSnapshot;
            }
                );
        }
    }
}
