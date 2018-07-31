using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public interface IMavenArtifactsRepository : IRepository<MavenArtifactEntity>
    {
        MavenArtifactEntity GetById(Guid repoId, string[] group, string artifactId, string version, string classifier);
        IEnumerable<MavenArtifactEntity> GetById(Guid repoId, string[] group, string artifactId, string version);
        IEnumerable<MavenArtifactEntity> GetVersionsByIdFirstIsRelease(Guid id, string[] group, string artifactId);
    }
}
