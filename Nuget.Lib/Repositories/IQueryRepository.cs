using NugetProtocol;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Repositories
{
    public interface IQueryRepository : IRepository<QueryEntity>
    {
        IEnumerable<QueryEntity> Query(Guid repoId, QueryModel model);
        QueryEntity GetByPackage(Guid repoId, string id, string version);
    }
}
