using Repositories;
using System;

namespace Maven.Repositories
{
    public interface IVersionedClassifiersRepository : IRepository<VersionedClassifierEntity>
    {
        VersionedClassifierEntity GetArtifactData(Guid repoId, string[] group, string artifactId, string version,string classifier);
    }
}
