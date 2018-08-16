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
    public class MavenSearchLastRepository : InMemoryRepository<MavenSearchLastEntity>, IMavenSearchLastRepository
    {
        private readonly IQueryToLinq _queryToLinq;

        public MavenSearchLastRepository(AppProperties properties, IQueryToLinq queryToLinq) : 
            base(properties)
        {
            this._queryToLinq = queryToLinq;
        }

        public MavenSearchLastEntity GetByArtifactId(Guid repoId, string artifactId, string groupId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MavenSearchLastEntity> Query(Guid repoId, SearchParam param)
        {
            throw new NotImplementedException();
        }
    }
}
