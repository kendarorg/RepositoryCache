using MavenProtocol.Apis;
using Repositories;
using System;
using System.Collections.Generic;

namespace Maven.Repositories
{
    public interface IMainArtifactsRepository : IRepository<MainArtifact>
    {
        IEnumerable<MainArtifact> Query(Guid repoId, SearchParam param, ITransaction transaction = null);
        MainArtifact GetMainArtifactById(Guid repoId,
            string[] group, string artifactId, string version, 
            ITransaction transaction = null);
        IEnumerable<MainArtifact> GetMainArtifactsById(Guid repoId,
            string[] group, string artifactId, string version, bool isSnapshot,
            ITransaction transaction = null);
    }
}
