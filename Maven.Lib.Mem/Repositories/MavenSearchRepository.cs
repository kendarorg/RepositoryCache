using Maven.Lib.Mem;
using Maven.Repositories;
using MavenProtocol.Apis;
using MultiRepositories;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven
{
    public class MavenSearchRepository : InMemoryRepository<OLDMavenSearchEntity>, OLDIMavenSearchRepository
    {
        private readonly IQueryToLinq _queryToLinq;

        public MavenSearchRepository(AppProperties properties, IQueryToLinq queryToLinq) : 
            base(properties)
        {
            this._queryToLinq = queryToLinq;
        }

        public IEnumerable<OLDMavenSearchEntity> GetByArtifactId(Guid repoId, string artifactId, string groupId)
        {
            return GetAll().Where(
                a => a.RepositoryId == repoId && a.ArtifactId == artifactId && a.Group == groupId);
        }

        public OLDMavenSearchEntity GetByArtifactIdVersion(Guid repoId, string[] group, string artifactId, string version)
        {
            return GetAll().FirstOrDefault(
                a => a.RepositoryId == repoId && a.ArtifactId == artifactId && a.Group ==string.Join(".", group) && a.Version==version);
        }

        public IEnumerable<OLDMavenSearchEntity> Query(Guid repoId, SearchParam param)
        {
            return _queryToLinq.Query(GetAll().AsQueryable(), repoId, param);
        }
    }
}
