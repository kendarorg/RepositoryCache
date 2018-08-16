using Maven.Lib.Mem;
using Maven.Repositories;
using MavenProtocol.Apis;
using MultiRepositories;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven
{
    public class MavenSearchRepository : InMemoryRepository<MavenSearchEntity>, IMavenSearchRepository
    {
        private readonly IQueryToLinq _queryToLinq;

        public MavenSearchRepository(AppProperties properties, IQueryToLinq queryToLinq) : 
            base(properties)
        {
            this._queryToLinq = queryToLinq;
        }

        public IEnumerable<MavenSearchEntity> GetByArtifactId(Guid repoId, string artifactId, string groupId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MavenSearchEntity> Query(Guid repoId, SearchParam param)
        {
            throw new NotImplementedException();
        }
    }
}
