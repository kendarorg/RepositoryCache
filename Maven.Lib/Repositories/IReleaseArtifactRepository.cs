using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.News
{
    /// <summary>
    /// Contains the MAIN versions (one snapshot and one not)
    /// </summary>
    public interface IReleaseArtifactRepository : IRepository<ReleaseVersion>
    {
        //The list of release artifacts
        
        ///ReleaseVersion GetReleaseVersion(Guid repoId, string[] group, string artifactId, bool isSnapshot, ITransaction transaction = null);
        
        
    }
}
