using MavenProtocol.Apis;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.News
{
    public interface IReleasePomRepository : IRepository<PomEntity>
    {
        PomEntity GetSinglePom(Guid repoId, string[] group, string artifactId, bool isSnapshot, ITransaction transaction = null);
        IEnumerable<PomEntity> Query(Guid repoId, SearchParam param);
    }
}
