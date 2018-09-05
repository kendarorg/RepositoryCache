using Repositories;
using System;
using System.Collections.Generic;

namespace Maven.Repositories
{
    public interface IVersionedClassifiersRepository : IRepository<VersionedClassifierEntity>
    {
        VersionedClassifierEntity GetSingleClassifierData(Guid repoId, string[] group, string artifactId, string version,bool isSnapshot, string classifier, string buildNumber, ITransaction transaction = null);
        IEnumerable<VersionedClassifierEntity> GetAllClassifiersData(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, string buildNumber, ITransaction transaction = null);
    }
}
