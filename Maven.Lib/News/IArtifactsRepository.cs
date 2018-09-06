using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.News
{
    public interface IArtifactsRepository : IRepository<ArtifactEntity>
    {
        IEnumerable<ArtifactEntity> GetSnapshotBuildArtifacts(Guid repoId, string[] group, string artifactId, string version, DateTime timestampToSeconds,string buildId,ITransaction transaction=null);
        IEnumerable<ArtifactEntity> GetAllArtifacts(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, ITransaction transaction = null);
    }
}
