using MavenProtocol.Apis;
using Repositories;
using System;
using System.Collections.Generic;

namespace Maven.Repositories
{
    public interface IVersionedArtifactRepository : IRepository<VersionedArtifactEntity>
    {
        IEnumerable<VersionedArtifactEntity> Query(Guid repoId, SearchParam param, ITransaction transaction = null);
        VersionedArtifactEntity GetSingleVersionedArtifact(Guid repoId, string[] group, string artifactId, string version,bool isSnapshot, string buildNumber,ITransaction transaction = null);
        IEnumerable<VersionedArtifactEntity> GetAllMainArtifacts(Guid repoId, string[] group, string artifactId,string version,bool isSnapshot, ITransaction transaction = null);
    }
}
