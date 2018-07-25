using System;
using System.Collections.Generic;
using System.Linq;
using Nuget.Repositories;
using NugetProtocol;
using Ioc;

namespace Nuget.Lib.Mem
{
    public interface IQueryToLinq:ISingleton
    {
        IEnumerable<QueryEntity> Query(IQueryable<QueryEntity> entities, Guid repoId, QueryModel query);
    }
}