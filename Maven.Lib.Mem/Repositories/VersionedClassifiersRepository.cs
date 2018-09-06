using MultiRepositories;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maven.Repositories
{
    public class VersionedClassifiersRepository : InMemoryRepository<SubArtifact>, ISubArtifactsRepository
    {
        public VersionedClassifiersRepository(AppProperties properties) : base(properties)
        {
        }

        public SubArtifact GetSingleClassifierData(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, string classifier,string build, ITransaction transaction = null)
        {
            return GetAll().FirstOrDefault(a => a.RepositoryId == repoId && a.Group == string.Join(".", group)
             && a.ArtifactId == artifactId && a.IsSnapshot == isSnapshot && a.Version == version && a.Classifer == classifier);
        }

        public IEnumerable<SubArtifact> GetAllClassifiersData(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, string build,ITransaction transaction = null)
        {
            return GetAll().Where(a => a.RepositoryId == repoId && a.Group == string.Join(".", group)
             && a.ArtifactId == artifactId && a.IsSnapshot == isSnapshot && a.Version == version);
        }
    }
}
