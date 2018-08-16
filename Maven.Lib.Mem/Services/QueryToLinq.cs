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
        public IEnumerable<MavenSearchLastEntity> Query(IQueryable<MavenSearchLastEntity> entities, Guid repoId, SearchParam query)
        {
            ParsedQuery pq = _queryBuilder.ParseQuery(query.Query);
            var ents = Query(entities, repoId, pq);
            foreach (var item in pq.Keys)
            {
                if (string.IsNullOrWhiteSpace(item.Value))
                {
                    continue;
                }
                switch (item.Key)
                {
                    case ("version"):
                        ents = ents.Where(r =>
                            ((MavenSearchLastEntity)r).VersionRelease == item.Value || ((MavenSearchLastEntity)r).VersionSnapshot == item.Value);
                        break;
                    case ("timestamp"):
                        ents = ents.Where(r =>
                            ((MavenSearchLastEntity)r).TimestampRelease.ToFileTime().ToString() == item.Value || ((MavenSearchLastEntity)r).TimestampSnapshot.ToFileTime().ToString() == item.Value);
                        break;
                }
            }

            foreach (var res in ents.Skip(query.Start).Take(query.Rows))
            {
                yield return (MavenSearchLastEntity)res;
            }
        }

        public IEnumerable<MavenSearchEntity> Query(IQueryable<MavenSearchEntity> entities, Guid repoId, SearchParam query)
        {
            ParsedQuery pq = _queryBuilder.ParseQuery(query.Query);
            var ents = Query(entities, repoId, pq);


            foreach (var res in ents.Skip(query.Start).Take(query.Rows))
            {
                yield return (MavenSearchEntity)res;
            }
        }

        private IQueryable<MavenBaseSearchEntity> Query(IQueryable<MavenBaseSearchEntity> entities, Guid repoId, ParsedQuery pq)
        {
            var result = entities.Where(a => a.RepositoryId == repoId).AsQueryable();

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
                    case ("packageid"):
                        result = result.Where(r => r.ArtifactId.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) == 0);
                        break;
                    case ("group"):
                        result = result.Where(r => r.Group.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) == 0);
                        break;
                    case ("version"):
                        result = result.Where(r => String.Compare(r.Version, item.Value, true) == 0);
                        break;
                    case ("packaging"):
                        result = result.Where(r => r.Type.IndexOf("|" + item.Value + "|", StringComparison.CurrentCultureIgnoreCase) >= 0);
                        break;
                    case ("classifier"):
                        result = result.Where(r => r.Classifiers.IndexOf("|" + item.Value + "|", StringComparison.CurrentCultureIgnoreCase) >= 0);
                        break;
                    case ("tags"):
                        result = result.Where(r => r.Tags.IndexOf(item.Value, StringComparison.CurrentCultureIgnoreCase) >= 0);
                        break;
                    case ("timestamp"):
                        result = result.Where(r => r.Timestamp.ToFileTime().ToString() == item.Value);
                        break;
                    default:
                        throw new NotSupportedException(item.Key);
                }
            }
            return result;
        }
    }
}
