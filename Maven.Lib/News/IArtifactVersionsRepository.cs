using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.News
{
    public interface IArtifactVersionsRepository : IRepository<ArtifactVersion>
    {
        //The list of release artifacts
        IEnumerable<ArtifactVersion> GetArtifacts(Guid repoId,string[] group, string artifactId,string version,bool isSnapshot, ITransaction transaction = null);
        ArtifactVersion GetSingleVersion(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, ITransaction transaction = null);
        ArtifactVersion GetSingleArtifact(Guid repoId, string[] group, string artifactId, string version, string classifier, string extension, bool isSnapshot, DateTime timestamp, string build);
        
    }
}
