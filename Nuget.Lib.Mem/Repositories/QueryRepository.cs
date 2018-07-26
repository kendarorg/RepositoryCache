using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NugetProtocol;
using MultiRepositories;
using Nuget.Lib.Mem;

namespace Nuget.Repositories
{
    public class QueryRepository : InMemoryRepository<QueryEntity>, IQueryRepository
    {
        private IQueryToLinq _queryToLinq;

        public QueryRepository(IQueryToLinq queryToLinq, AppProperties properties) : base(properties)
        {
            _queryToLinq = queryToLinq;
        }
        
        public QueryEntity GetByPackage(Guid repoId, string id)
        {
            return GetAll().FirstOrDefault(a => a.Listed && a.RepositoryId == repoId && a.PackageId == id);
        }

        public IEnumerable<QueryEntity> Query(Guid repoId, QueryModel model)
        {
            return _queryToLinq.Query(GetAll().AsQueryable(), repoId, model).Where(a=>a.Listed);
        }
    }
}
