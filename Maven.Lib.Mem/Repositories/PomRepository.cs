using Maven.News;
using MultiRepositories;
using Repositories;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public class PomRepository : InMemoryRepository<PomEntity>, IPomRepository
    {
        public PomRepository(AppProperties properties) : base(properties)
        {
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
    }
}
