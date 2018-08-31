using MavenProtocol.Apis;
using Repositories;
using System;
using System.Collections.Generic;

namespace Maven.Repositories
{
    public interface IReleaseArtifacts : IRepository<ReleaseEntity>
    {
        IEnumerable<ReleaseEntity> Query(Guid repoId, SearchParam param, ITransaction transaction);
        ReleaseEntity GetByArtifact(Guid repoId,string[] group, string artifactId, ITransaction transaction);
    }
}
