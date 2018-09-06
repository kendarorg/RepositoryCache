using System;
using System.Collections.Generic;
using System.Linq;
//using Maven.Repositories;
using MavenProtocol;
using Ioc;
using MavenProtocol.Apis;
using Maven.Repositories;

namespace Maven.Lib.Mem
{
    public interface IQueryToLinq : ISingleton
    {
        IEnumerable<MainArtifact> Query(IQueryable<MainArtifact> entities, Guid repoId, SearchParam query);
        IEnumerable<ReleaseArtifactEntity> Query(IQueryable<ReleaseArtifactEntity> entities, Guid repoId, SearchParam query);
    }
}