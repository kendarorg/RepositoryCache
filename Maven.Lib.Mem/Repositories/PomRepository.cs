using Maven.Lib.Mem;
using Maven.News;
using MavenProtocol.Apis;
using MultiRepositories;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public class PomRepository : InMemoryRepository<PomEntity>, IPomRepository
    {
        private readonly IQueryToLinq _queryToLinq;

        public PomRepository(AppProperties properties, IQueryToLinq queryToLinq) : base(properties)
        {
            this._queryToLinq = queryToLinq;
        }

        public IEnumerable<PomEntity> GetPomForVersion(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot)
        {
            return GetAll().Where(a =>
            {
                return a.Group == string.Join(".", group) && a.ArtifactId == artifactId && a.Version == version && a.IsSnapshot == isSnapshot;
            });
        }

        public PomEntity GetSinglePom(Guid repoId, string[] group, string artifactId, string version,  bool isSnapshot, DateTime timestamp, string build, ITransaction transaction = null)
        {
            return GetAll().FirstOrDefault(a =>
            {
                var checkTimestamp = !string.IsNullOrWhiteSpace(a.Build) && timestamp.Year > 1 ? a.Timestamp.ToFileTime() == timestamp.ToFileTime() : true;
                return a.Group == string.Join(".", group) && a.ArtifactId == artifactId && a.Version == version && a.IsSnapshot == isSnapshot &&
                     a.Build == build && checkTimestamp;
            });
        }

        public IEnumerable<PomEntity> Query(Guid repoId, SearchParam param)
        {
            return _queryToLinq.Query(GetAll().AsQueryable(), repoId, param);
        }
    }
}
