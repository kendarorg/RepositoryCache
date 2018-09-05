using MultiRepositories;
using Repositories;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public class ArtifactRepository : InMemoryRepository<ArtifactEntity>, IArtifactRepository
    {
        public ArtifactRepository(AppProperties properties) : base(properties)
        {
        }

        public ArtifactEntity GetMetadata(Guid repoId, string[] group, string artifactId,string version=null, ITransaction transaction = null)
        {
            return GetAll().FirstOrDefault(a => a.RepositoryId == repoId && a.Group == string.Join(".", group)
             && a.ArtifactId == artifactId );
        }
    }
}
