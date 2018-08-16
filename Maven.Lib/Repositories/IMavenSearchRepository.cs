using MavenProtocol.Apis;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public interface IMavenSearchRepository : IRepository<MavenSearchEntity>
    {
        IEnumerable<MavenSearchEntity> Query(Guid repoId, SearchParam param);

        IEnumerable<MavenSearchEntity> GetByArtifactId(Guid repoId, string artifactId, string groupId);
    }
}
