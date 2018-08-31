using MavenProtocol.Apis;
using Repositories;
using System;
using System.Collections.Generic;

namespace Maven.Repositories
{
    public interface IVersionedArtifactRepository : IRepository<VersionedArtifactEntity>
    {
        IEnumerable<ReleaseEntity> Query(Guid repoId, SearchParam param);
        VersionedArtifactEntity GetArtifactData(Guid repoId, string[] group, string artifactId, string version);
    }
}
