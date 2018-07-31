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
        public MavenSearchRepository(AppProperties properties) : 
            base(properties)
        {
        }

        public IEnumerable<MavenSearchEntity> Query(Guid repoId, SearchParam param)
        {
            throw new NotImplementedException();
        }
    }
}
