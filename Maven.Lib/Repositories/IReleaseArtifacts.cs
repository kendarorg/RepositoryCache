using MavenProtocol.Apis;
using Repositories;
using System;
using System.Collections.Generic;

namespace Maven.Repositories
{
    public interface IReleaseArtifacts : IRepository<ReleaseEntity>
    {
        IEnumerable<ReleaseEntity> Query(Guid repoId, SearchParam param);
        ReleaseEntity GetByArtifact(Guid repoId,string[] group, string artifactId);
    }
}
