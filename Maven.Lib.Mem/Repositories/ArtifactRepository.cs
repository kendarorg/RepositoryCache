using MultiRepositories;
using Repositories;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public class ArtifactRepository : InMemoryRepository<MavenMetadataEntity>, IMetadataRepository
    {
        public ArtifactRepository(AppProperties properties) : base(properties)
        {
        }

        public MavenMetadataEntity GetMetadata(Guid repoId, string[] group, string artifactId,string version=null, ITransaction transaction = null)
        {
            return GetAll().FirstOrDefault(a => a.RepositoryId == repoId && a.Group == string.Join(".", group)
             && a.ArtifactId == artifactId );
        }
    }
}
