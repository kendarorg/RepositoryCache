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
        private QueryToLinq _queryToLinq;

        public QueryRepository(QueryToLinq queryToLinq, AppProperties properties) : base(properties)
        {
            _queryToLinq = queryToLinq;
        }

        public IEnumerable<QueryEntity> Query(Guid repoId, QueryModel model)
        {
            return _queryToLinq.Query(GetAll().AsQueryable(), repoId, model);
        }
    }
}
