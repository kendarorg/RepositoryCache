using Maven.Lib.Mem;
using Maven.News;
using MavenProtocol.Apis;
using MultiRepositories;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public class ReleasePomRepository : InMemoryRepository<PomEntity>, IReleasePomRepository
    {
        private readonly IQueryToLinq _queryToLinq;

        public ReleasePomRepository(AppProperties properties, IQueryToLinq queryToLinq) : base(properties)
        {
            this._queryToLinq = queryToLinq;
        }

        public PomEntity GetSinglePom(Guid repoId, string[] group, string artifactId, bool isSnapshot,ITransaction transaction = null)
        {
            return GetAll().FirstOrDefault(a =>
            {
                return a.Group == string.Join(".", group) && a.ArtifactId == artifactId 
                    && a.IsSnapshot == isSnapshot;
            });
        }

        public IEnumerable<PomEntity> Query(Guid repoId, SearchParam param)
        {
            return _queryToLinq.Query(GetAll().AsQueryable(), repoId, param);
        }
    }
}
