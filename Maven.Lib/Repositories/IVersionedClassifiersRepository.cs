using Repositories;
using System;
using System.Collections.Generic;

namespace Maven.Repositories
{
    public interface IVersionedClassifiersRepository : IRepository<VersionedClassifierEntity>
    {
        VersionedClassifierEntity GetArtifactData(Guid repoId, string[] group, string artifactId, string version,bool isSnapshot, string classifier, string buildNumber, ITransaction transaction = null);
        IEnumerable<VersionedClassifierEntity> GetArtifactData(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, string buildNumber, ITransaction transaction = null);
    }
}
