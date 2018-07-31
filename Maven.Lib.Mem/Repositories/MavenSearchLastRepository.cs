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
    public class MavenSearchLastRepository : InMemoryRepository<MavenSearchEntity>, IMavenSearchLastRepository
    {
        public MavenSearchLastRepository(AppProperties properties) : 
            base(properties)
        {
        }

        public IEnumerable<MavenSearchEntity> Query(Guid repoId, SearchParam param)
        {
            throw new NotImplementedException();
        }
    }
}
