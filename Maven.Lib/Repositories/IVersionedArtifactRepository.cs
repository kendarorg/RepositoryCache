using MavenProtocol.Apis;
using Repositories;
using System;
using System.Collections.Generic;

namespace Maven.Repositories
{
    public interface IVersionedArtifactRepository : IRepository<VersionedArtifactEntity>
    {
        IEnumerable<VersionedArtifactEntity> Query(Guid repoId, SearchParam param);
        VersionedArtifactEntity GetArtifactData(Guid repoId, string[] group, string artifactId, string version,bool isSnapshot);
        IEnumerable<VersionedArtifactEntity> GetArtifactData(Guid repoId, string[] group, string artifactId);
    }
}
