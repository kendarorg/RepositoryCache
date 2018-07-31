using Maven.Repositories;
using MavenProtocol;
using MavenProtocol.Apis;
using MultiRepositories.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Apis
{
    public class MavenSearchService : IMavenSearch
    {
        private readonly IRepositoryEntitiesRepository _repository;
        private readonly IServicesMapper _servicesMapper;
        private readonly IMavenSearchRepository _queryRepository;

        public MavenSearchService(IRepositoryEntitiesRepository repositoryEntitiesRepository,
            IServicesMapper servicesMapper,
            IMavenSearchRepository mavenSearchRepository)
        {
            this._repository = repositoryEntitiesRepository;
            this._servicesMapper = servicesMapper;
            this._queryRepository = mavenSearchRepository;
        }
        public SearchResult Search(Guid repoId, SearchParam param)
        {
            var repo = _repository.GetById(repoId);
            var maxSize = _servicesMapper.MaxQueryPage(repo.Id);

            var reqHeader = new Dictionary<string, string>();
            AddIfPresent(reqHeader, "q", param.Query);
            AddIfPresent(reqHeader, "wt", param.Wt);
            if (param.Rows < 0 || param.Rows > maxSize)
            {
                param.Rows = maxSize;
            }
            AddIfPresent(reqHeader, "rows", param.Rows.ToString());

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var docs = new List<ResponseDoc>();

            var max = 0;
            foreach (var item in _queryRepository.Query(repoId, param))
            {
                if (max >= param.Rows)
                {
                    break;
                }
                docs.Add(BuildResponse(item));
                max++;
            }

            var numfound = docs.Count;
            stopwatch.Stop();

            return new SearchResult(
                new ResponseHeader(0, (int)stopwatch.ElapsedMilliseconds / 1000, reqHeader),
                new ResponseContent(numfound, param.Start, docs));
        }

        private void AddIfPresent(Dictionary<string, string> reqHeader, string id, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                reqHeader.Add(id, value);
            }
        }

        private ResponseDoc BuildResponse(MavenSearchEntity item)
        {
            throw new NotImplementedException();
        }
    }
}
