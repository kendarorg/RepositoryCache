using MavenProtocol.Apis;
using Repositories;
using System;
using System.Collections.Generic;

namespace Maven.Repositories
{
    public interface IReleaseArtifactsRepository : IRepository<ReleaseArtifactEntity>
    {
        IEnumerable<ReleaseArtifactEntity> Query(Guid repoId, SearchParam param, ITransaction transaction = null);
        ReleaseArtifactEntity GetById(Guid repoId, string[] group, string artifactId, ITransaction transaction = null);
    }
}
