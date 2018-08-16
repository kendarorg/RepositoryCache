using MavenProtocol.Apis;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public interface IMavenSearchLastRepository : IRepository<MavenSearchLastEntity>
    {
        IEnumerable<MavenSearchLastEntity> Query(Guid repoId, SearchParam param);
        MavenSearchLastEntity GetByArtifactId(Guid repoId, string artifactId, string groupId);
    }
}
