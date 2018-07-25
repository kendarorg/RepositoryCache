using Newtonsoft.Json;
//.Models;
//.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories.Repositories;
using MultiRepositories.Service;
using MultiRepositories;
using NugetProtocol;

namespace Nuget.Controllers
{
    public class V3_Query : ForwardRestApi
    {
        private IRepositoryEntitiesRepository _reps;
        private ISearchQueryService _searchQueryService;
        private IServicesMapper _servicesMapper;

        public V3_Query(
            ISearchQueryService searchQueryService, AppProperties properties, IRepositoryEntitiesRepository reps,
            IServicesMapper servicesMapper,params string[]paths) :
            base(properties,  null,paths)
        {
            _reps = reps;
            _searchQueryService = searchQueryService;
            _servicesMapper = servicesMapper;
            SetHandler(Handle);
        }

        private SerializableResponse Handle(SerializableRequest localRequest)
        {
            QueryResult result = null;
            var repo = _reps.GetByName(localRequest.PathParams["repo"]);
            var qm = new QueryModel()
            {
                Query = localRequest.QueryParams.ContainsKey("q") ? localRequest.QueryParams["q"] : "",
                PreRelease = localRequest.QueryParams.ContainsKey("prerelease") ? bool.Parse(localRequest.QueryParams["prerelease"]) : false,
                SemVerLevel = localRequest.QueryParams.ContainsKey("semverlevel") ? localRequest.QueryParams["semverlevel"] : "2.0.0",
                Skip = localRequest.QueryParams.ContainsKey("skip") ? int.Parse(localRequest.QueryParams["skip"]) : 0,
                Take = localRequest.QueryParams.ContainsKey("take") ? int.Parse(localRequest.QueryParams["take"]) : 26

            };
            
            if (repo.Mirror)
            {
                try
                {
                    result = SearchRemote(localRequest, repo);
                }
                catch (Exception)
                {

                }
            }
            if (result == null)
            {
                result = _searchQueryService.Query(repo.Id, qm);
            }
            return JsonResponse(result);
        }

        private QueryResult SearchRemote(SerializableRequest localRequest, RepositoryEntity repo)
        {
            QueryResult result;
            var remoteRequest = localRequest.Clone();
            var convertedUrl = _servicesMapper.ToNuget(repo.Id, localRequest.Protocol + "://" + localRequest.Host + localRequest.Url);
            remoteRequest.Headers["Host"] = new Uri(convertedUrl).Host;

            var path = localRequest.ToLocalPath("index.json");
            var remoteRes = RemoteRequest(convertedUrl, remoteRequest);
            
            result = JsonConvert.DeserializeObject<QueryResult>(Encoding.UTF8.GetString(remoteRes.Content));
            result.OContext.OBase = _servicesMapper.FromNuget(repo.Id, result.OContext.OBase);
            foreach (var item in result.Data)
            {
                item.Id = _servicesMapper.FromNuget(repo.Id, item.Id);
                item.OId = _servicesMapper.FromNuget(repo.Id, item.OId);
                item.Registration = _servicesMapper.FromNuget(repo.Id, item.Registration);
                foreach (var ver in item.Versions)
                {
                    ver.OId = _servicesMapper.FromNuget(repo.Id, ver.OId);
                }
            }

            return result;
        }
    }
}
