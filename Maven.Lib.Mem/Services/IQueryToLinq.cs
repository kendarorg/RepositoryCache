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
        IEnumerable<VersionedArtifactEntity> Query(IQueryable<VersionedArtifactEntity> entities, Guid repoId, SearchParam query);
        IEnumerable<ReleaseEntity> Query(IQueryable<ReleaseEntity> entities, Guid repoId, SearchParam query);
    }
}