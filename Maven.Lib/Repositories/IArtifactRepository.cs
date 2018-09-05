using Repositories;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public interface IArtifactRepository:IRepository<ArtifactEntity>
    {
        ArtifactEntity GetMetadata(Guid repoId, string[] group, string artifactId,string version=null,bool isSnapshot=false, ITransaction transaction=null);
    }
}
