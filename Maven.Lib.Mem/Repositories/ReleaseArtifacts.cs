using Maven.Lib.Mem;
using MavenProtocol.Apis;
using MultiRepositories;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maven.Repositories
{
    public class ReleaseArtifacts : InMemoryRepository<ReleaseEntity>, IReleaseArtifacts
    {
        private readonly IQueryToLinq _queryToLinq;

        public ReleaseArtifacts(AppProperties properties, IQueryToLinq queryToLinq) : base(properties)
        {
            this._queryToLinq = queryToLinq;
        }

        public ReleaseEntity GetByArtifact(Guid repoId, string[] group, string artifactId, ITransaction transaction = null)
        {
            return GetAll().FirstOrDefault(a => a.RepositoryId == repoId && a.Group == string.Join(".", group)
             && a.ArtifactId == artifactId );
        }

        public IEnumerable<ReleaseEntity> Query(Guid repoId, SearchParam param, ITransaction transaction = null)
        {
            return _queryToLinq.Query(GetAll().AsQueryable(), repoId, param);
        }
    }
}
