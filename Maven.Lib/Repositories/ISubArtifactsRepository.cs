using Repositories;
using System;
using System.Collections.Generic;

namespace Maven.Repositories
{
    public interface ISubArtifactsRepository : IRepository<SubArtifact>
    {
        SubArtifact GetSubArtifactById(Guid repoId, string[] group, string artifactId, string version,string classifier, bool isSnapshot, string buildNumber, ITransaction transaction = null);
        IEnumerable<SubArtifact> GetSubArtifactsForArtifactId(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, string buildNumber, ITransaction transaction = null);
        IEnumerable<SubArtifact> GetSubArtifactsByVersion(Guid repoId, string[] group, string artifactId, string version, ITransaction transaction = null);
    }
}
