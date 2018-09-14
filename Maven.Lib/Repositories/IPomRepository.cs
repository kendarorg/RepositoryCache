using MavenProtocol.Apis;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.News
{
    public interface IPomRepository : IRepository<PomEntity>
    {
        PomEntity GetSinglePom(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, 
            DateTime timestamp, string build, ITransaction transaction = null);
        IEnumerable<PomEntity> Query(Guid repoId, SearchParam param);
        IEnumerable<PomEntity> GetPomForVersion(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot);
    }
}
