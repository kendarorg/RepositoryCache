using Maven.Lib.Mem;
using MavenProtocol.Apis;
using MultiRepositories;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maven.Repositories
{
    public class VersionedArtifactRepository : InMemoryRepository<VersionedArtifactEntity>, IVersionedArtifactRepository
    {
        private readonly IQueryToLinq _queryToLinq;

        public VersionedArtifactRepository(AppProperties properties, IQueryToLinq queryToLinq) : base(properties)
        {
            this._queryToLinq = queryToLinq;
        }

        public VersionedArtifactEntity GetArtifactData(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, ITransaction transaction = null)
        {
            return GetAll().FirstOrDefault(a => a.RepositoryId == repoId && a.Group == string.Join(".", group)
                && a.ArtifactId == artifactId && a.IsSnapshot==isSnapshot && a.Version==version);
        }

        public IEnumerable<VersionedArtifactEntity> GetArtifactData(Guid repoId, string[] group, string artifactId, ITransaction transaction = null)
        {
            return GetAll().Where(a => a.RepositoryId == repoId && a.Group == string.Join(".", group)
                && a.ArtifactId == artifactId);
        }

        public IEnumerable<VersionedArtifactEntity> Query(Guid repoId, SearchParam param, ITransaction transaction = null)
        {
            return _queryToLinq.Query(GetAll().AsQueryable(), repoId, param);
        }
    }
}
