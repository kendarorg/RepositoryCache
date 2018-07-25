using Ioc;
using Nuget.Repositories;
using Nuget.Services;
using NugetProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Lib.Mem
{
    public class QueryToLinq : ISingleton, IQueryToLinq
    {
        private IQueryBuilder _queryBuilder;

        public QueryToLinq(IQueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
        }
        public IEnumerable<QueryEntity> Query(IQueryable<QueryEntity> entities,Guid repoId, QueryModel query)
        {
            var result = entities.Where(a => a.RepositoryId == repoId);
            ParsedQuery pq = _queryBuilder.ParseQuery(query.Query);
            if (pq.FreeText.Any())
            {
                result = result.Where(r => pq.FreeText.Any(a => r.FreeText.IndexOf(a, StringComparison.CurrentCultureIgnoreCase) >= 0));
            }
            if (!query.PreRelease)
            {
                result = result.Where(a => a.HasRelease);
            }
            foreach (var item in pq.Keys)
            {
                switch (item.Key)
                {
                    case ("id"):
                        result = result.Where(r => r.PackageId.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) >= 0);
                        break;
                    case ("title"):
                        result = result.Where(r => r.Title.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) >= 0);
                        break;
                    case ("packageid"):
                        result = result.Where(r => r.PackageId == item.Value);
                        break;
                    case ("tags"):
                        result = result.Where(r => r.Tags.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) >= 0);
                        break;
                    case ("author"):
                        result = result.Where(r => r.Author.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) >= 0);
                        break;
                    case ("description"):
                        result = result.Where(r => r.Description.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) >= 0);
                        break;
                    case ("summary"):
                        result = result.Where(r => r.Summary.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) >= 0);
                        break;
                    case ("owner"):
                        result = result.Where(r => r.Owner.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) >= 0);
                        break;
                    default:
                        throw new NotSupportedException(item.Key);
                }
            }
            return result.OrderByDescending(a => a.TotalDownloads).Skip(query.Skip);
        }
    }
}
