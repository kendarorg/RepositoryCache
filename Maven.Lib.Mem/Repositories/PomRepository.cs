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

        public PomEntity GetSinglePom(Guid repoId, string[] group, string artifactId, string version, string classifier, string extension, bool isSnapshot, DateTime timestamp, string build, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }
    }
}
