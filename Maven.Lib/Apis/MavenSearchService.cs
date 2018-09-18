//using Maven.Repositories;
using Maven.News;
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
        private readonly IPomRepository _mavenSearchRepository;
        private readonly IReleasePomRepository _releasePomRepository;

        public MavenSearchService(IRepositoryEntitiesRepository repositoryEntitiesRepository,
            IServicesMapper servicesMapper,
            IPomRepository mavenSearchRepository, IReleasePomRepository releasePomRepository)
        {
            this._repository = repositoryEntitiesRepository;
            this._servicesMapper = servicesMapper;
            this._mavenSearchRepository = mavenSearchRepository;
            this._releasePomRepository = releasePomRepository;
        }
        public SearchResult Search(Guid repoId, SearchParam param)
        {
            var repo = _repository.GetById(repoId);
            var maxSize = _servicesMapper.MaxQueryPage(repo.Id);

            var reqHeader = LoadParameters(param, maxSize);

            var stopwatch = new Stopwatch();
            var docs = new List<ResponseDoc>();
            stopwatch.Start();

            var max = 0;

            if (param.Core != "gav")
            {
                max = GetAllVersions(repoId, param, docs, max);
            }
            else
            {
                max = GetReleasesOnly(repoId, param, docs, max);
            }
            stopwatch.Stop();
            var numfound = docs.Count;
            return new SearchResult(
                new ResponseHeader(0, (int)stopwatch.ElapsedMilliseconds / 1000, reqHeader),
                new ResponseContent(numfound, param.Start, docs));
        }

        private int GetReleasesOnly(Guid repoId, SearchParam param, List<ResponseDoc> docs, int max)
        {
            var result = _mavenSearchRepository.Query(repoId, param);
            foreach (var item in result)
            {
                if (max >= param.Rows)
                {
                    break;
                }
                docs.Add(BuildResponse(item));
                max++;
            }

            return max;
        }

        private int GetAllVersions(Guid repoId, SearchParam param, List<ResponseDoc> docs, int max)
        {
            var result = _releasePomRepository.Query(repoId, param);
            foreach (var item in result)
            {
                if (max >= param.Rows)
                {
                    break;
                }
                docs.Add(BuildResponse(item));
                max++;
            }

            return max;
        }

        private Dictionary<string, string> LoadParameters(SearchParam param, int maxSize)
        {
            var reqHeader = new Dictionary<string, string>();

            AddIfPresent(reqHeader, "q", param.Query);
            AddIfPresent(reqHeader, "wt", param.Wt);
            AddIfPresent(reqHeader, "core", param.Core);
            if (string.IsNullOrWhiteSpace(param.Wt) || (param.Wt != "json" && param.Wt != "xml"))
            {
                param.Wt = "json";
            }

            if (param.Rows < 0 || param.Rows > maxSize)
            {
                param.Rows = maxSize;
            }
            AddIfPresent(reqHeader, "rows", param.Rows.ToString());
            return reqHeader;
        }

        private void AddIfPresent(Dictionary<string, string> reqHeader, string id, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                reqHeader.Add(id, value);
            }
        }

        private ResponseDoc BuildResponse(PomEntity item)
        {
            var snap = item.IsSnapshot ? "-SNAPSHOT" : "";
            var id = item.Group + ":" + item.ArtifactId + ":" + item.Version + snap;
            List<string> typeAndExt = null;
            List<string> tags = null;
            if (!string.IsNullOrWhiteSpace(item.Classifiers))
            {
                typeAndExt = item.Classifiers.Split('|').Select(a => a.Trim('|')).
                    Where(b => !string.IsNullOrWhiteSpace(b)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(item.Tags))
            {
                tags = item.Tags.Split('|').Select(a => a.Trim('|')).
                    Where(b => !string.IsNullOrWhiteSpace(b)).ToList();
            }
            return new ResponseDoc(
                id, item.Group, item.ArtifactId, item.Version + snap,
                item.Timestamp.Year > 1 ? item.Timestamp.ToFileTime() : 0,
                typeAndExt,
                tags);
        }
    }
}
