using System;
using System.Collections.Generic;
using System.Linq;
//using Maven.Repositories;
using MavenProtocol;
using Ioc;
using MavenProtocol.Apis;
using Maven.Repositories;
using Maven.News;

namespace Maven.Lib.Mem
{
    public interface IQueryToLinq : ISingleton
    {
        //IEnumerable<MainArtifact> Query(IQueryable<MainArtifact> entities, Guid repoId, SearchParam query);
        IEnumerable<PomEntity> Query(IQueryable<PomEntity> entities, Guid repoId, SearchParam query);
    }
}