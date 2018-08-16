using System;
using System.Collections.Generic;
using System.Linq;
using Maven.Repositories;
using MavenProtocol;
using Ioc;
using MavenProtocol.Apis;

namespace Maven.Lib.Mem
{
    public interface IQueryToLinq:ISingleton
    {
        IEnumerable<MavenSearchLastEntity> Query(IQueryable<MavenSearchLastEntity> entities, Guid repoId, SearchParam query);
        IEnumerable<MavenSearchEntity> Query(IQueryable<MavenSearchEntity> entities, Guid repoId, SearchParam query);
    }
}