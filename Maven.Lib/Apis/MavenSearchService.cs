//using Maven.Repositories;
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
#if FALSE
    public class MavenSearchService : IMavenSearch
    {
        private readonly IRepositoryEntitiesRepository _repository;
        private readonly IServicesMapper _servicesMapper;
        private readonly OLDIMavenSearchRepository _mavenSearchRepository;
        private readonly OLDIMavenSearchLastRepository _mavenSearchLastRepository;

        public MavenSearchService(IRepositoryEntitiesRepository repositoryEntitiesRepository,
            IServicesMapper servicesMapper,
            OLDIMavenSearchRepository mavenSearchRepository,
            OLDIMavenSearchLastRepository mavenSearchLastRepository)
        {
            this._repository = repositoryEntitiesRepository;
            this._servicesMapper = servicesMapper;
            this._mavenSearchRepository = mavenSearchRepository;
            this._mavenSearchLastRepository = mavenSearchLastRepository;
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

            if (param.Wt == "gav")
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
            }
            else
            {
                var result = _mavenSearchLastRepository.Query(repoId, param);
                foreach (var item in result)
                {
                    if (max >= param.Rows)
                    {
                        break;
                    }
                    docs.Add(BuildResponse(item));
                    max++;
                }
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

        private ResponseDoc BuildResponse(OLDMavenSearchLastEntity item)
        {
            var id = item.Group + ":" + item.ArtifactId + ":" + item.Version;
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
                id, item.Group, item.ArtifactId, item.Version,
                item.Timestamp.ToFileTime(),
                typeAndExt,
                tags);
        }

        private ResponseDoc BuildResponse(OLDMavenSearchEntity item)
        {
            var id = item.Group + ":" + item.ArtifactId + ":" + item.Version;
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
                id, item.Group, item.ArtifactId, item.Version,
                item.Timestamp.ToFileTime(),
                typeAndExt,
                tags);
        }
    }
#endif
}
