using MultiRepositories;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maven.Repositories
{
    public class VersionedClassifiersRepository : InMemoryRepository<VersionedClassifierEntity>, IVersionedClassifiersRepository
    {
        public VersionedClassifiersRepository(AppProperties properties) : base(properties)
        {
        }

        public VersionedClassifierEntity GetArtifactData(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, string classifier, ITransaction transaction = null)
        {
            return GetAll().FirstOrDefault(a => a.RepositoryId == repoId && a.Group == string.Join(".", group)
             && a.ArtifactId == artifactId && a.IsSnapshot == isSnapshot && a.Version == version && a.Classifer == classifier);
        }

        public IEnumerable<VersionedClassifierEntity> GetArtifactData(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, ITransaction transaction = null)
        {
            return GetAll().Where(a => a.RepositoryId == repoId && a.Group == string.Join(".", group)
             && a.ArtifactId == artifactId && a.IsSnapshot == isSnapshot && a.Version == version);
        }
    }
}
