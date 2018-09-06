using Maven.News;
using MultiRepositories;
using Repositories;
using System;

namespace Maven.Repositories
{
    public class ReleaseArtifactRepository : InMemoryRepository<ReleaseVersion>, IReleaseArtifactRepository
    {
        public ReleaseArtifactRepository(AppProperties properties) : base(properties)
        {
        }

        public ReleaseVersion GetSingleVersion(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }
    }
}
