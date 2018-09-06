using Repositories;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public interface IMetadataRepository:IRepository<MavenMetadataEntity>
    {
        MavenMetadataEntity GetMetadataById(Guid repoId, string[] group, string artifactId, ITransaction transaction=null);
        MavenMetadataEntity GetSubMetadataById(Guid repoId, string[] group, string artifactId,bool isSnapshot,string build, ITransaction transaction = null);
    }
}
