using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.News
{
    public interface IMetadataRepository : IRepository<MetadataEntity>
    {
        MetadataEntity GetArtifactMetadata(Guid repoId, string[] group, string artifactId, ITransaction transaction = null);
        MetadataEntity GetArtifactVersionMetadata(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, ITransaction transaction = null);
        IEnumerable<MetadataEntity> GetVersions(Guid repoId, string[] group, string artifactId, ITransaction transaction = null);
    }
}
