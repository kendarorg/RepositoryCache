using Ioc;
using Maven.Repositories;
using Maven.Services;
using MavenProtocol.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Lib.Mem
{
    public class QueryToLinq : ISingleton, IQueryToLinq
    {
        private IQueryBuilder _queryBuilder;

        public QueryToLinq(IQueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
        }
        public IEnumerable<MavenSearchLastEntity> Query(IQueryable<MavenSearchLastEntity> entities,Guid repoId, SearchParam query)
        {
            throw new NotImplementedException();
#if TODO
            var result = entities.Where(a => a.RepositoryId == repoId);
            ParsedQuery pq = _queryBuilder.ParseQuery(query.Query);
            if (pq.FreeText.Any())
            {
                result = result.Where(r => pq.FreeText.Any(a => r.FreeText.IndexOf(a, StringComparison.CurrentCultureIgnoreCase) >= 0));
            }
            foreach (var item in pq.Keys)
            {
                if (string.IsNullOrWhiteSpace(item.Value))
                {
                    continue;
                }
                switch (item.Key)
                {
                    case ("id"):
                        result = result.Where(r => r.ArtifactId.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) >= 0);
                        break;
                    default:
                        throw new NotSupportedException(item.Key);
                }
            }
            
            return result.Skip(query.Start);
#endif
        }

        public IEnumerable<MavenSearchEntity> Query(IQueryable<MavenSearchEntity> entities, Guid repoId, SearchParam query)
        {
            throw new NotImplementedException();
        }
    }
}
