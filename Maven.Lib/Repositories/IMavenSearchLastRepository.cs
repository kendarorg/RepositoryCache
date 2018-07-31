using MavenProtocol.Apis;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public interface IMavenSearchLastRepository : IRepository<MavenSearchEntity>
    {
        IEnumerable<MavenSearchEntity> Query(Guid repoId, SearchParam param);
    }
}
