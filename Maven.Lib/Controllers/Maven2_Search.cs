using MultiRepositories.Repositories;
using MultiRepositories.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;
using System.IO;
using Newtonsoft.Json;
using MavenProtocol.Apis;
using Maven.Services;
using MavenProtocol;
using System.Xml.Serialization;
using System.Xml;

namespace Maven.Controllers
{
    public class Maven2_Search : ForwardRestApi
    {
        private readonly IMetadataApi _interfaceService;
        private readonly IMavenSearch _mavenSearch;
        private readonly IServicesMapper _servicesMapper;
        private readonly Guid _repoId;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IRequestParser _requestParser;

        public Maven2_Search(Guid repoId, AppProperties properties,
            IRepositoryEntitiesRepository repositoryEntitiesRepository, IRequestParser requestParser,
            IMetadataApi interfaceService, IMavenSearch mavenSearch, IServicesMapper servicesMapper, params string[] paths)
            : base(properties, null, paths)
        {
            _interfaceService = interfaceService;
            this._mavenSearch = mavenSearch;
            this._servicesMapper = servicesMapper;
            this._repoId = repoId;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._requestParser = requestParser;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest arg)
        {
            var idx = _requestParser.Parse(arg);
            var repo = _repositoryEntitiesRepository.GetById(_repoId);
            idx.RepoId = _repoId;
            var sp = new SearchParam();
            sp.Wt = "json";
            if (arg.QueryParams.ContainsKey("q")) sp.Query = arg.QueryParams["q"];
            if (arg.QueryParams.ContainsKey("rows")) sp.Rows = int.Parse(arg.QueryParams["rows"]);
            if (arg.QueryParams.ContainsKey("skip")) sp.Start = int.Parse(arg.QueryParams["skip"]);
            if (arg.QueryParams.ContainsKey("core")) sp.Core = arg.QueryParams["core"];
            if (arg.QueryParams.ContainsKey("wt")) sp.Wt = arg.QueryParams["wt"];
            var reqWt = sp.Wt;
            if (sp.Rows == 0)
            {
                sp.Rows = _servicesMapper.MaxQueryPage(_repoId);
            }
            SearchResult result = null;
            if (repo.Mirror && _properties.IsOnline(arg))
            {
                try
                {
                    var baseStandard = "";
                    var baseurls = arg.Url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (baseurls.Length >= 1)
                    {
                        baseStandard = "/" + string.Join("/", baseurls.Take(baseurls.Length - 2));
                    }
                    arg.QueryParams["wt"] = "json";
                    result = ExploreRemote(arg, repo, idx, arg.Url);
                    
                }
                catch (InconsistentRemoteDataException)
                {
                    throw;
                }
                catch (Exception)
                {

                }
            }
            if (result == null)
            {
                result = _mavenSearch.Search(_repoId, sp);
            }
            //if (reqWt == "json")
            {
                return JsonResponse(result);
            }
            #if NOPE
            else
            {
                var data =/* @"{
  '?xml': {
    '@version': '1.0',
    '@standalone': 'no'
  },"+*/JsonConvert.SerializeObject(result);// +"}";

                XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(data, "XmlResult");
                return new SerializableResponse
                {
                    Content = Encoding.UTF8.GetBytes(doc.InnerXml),
                    ContentType = "application /xml"
                };
            }
#endif
        }

        private SearchResult ExploreRemote(SerializableRequest localRequest, RepositoryEntity repo, MavenIndex idx, string url)
        {
            var remoteRequest = localRequest.Clone();
            var convertedUrl = _servicesMapper.ToMavenSearch(repo.Id, idx, false);
            remoteRequest.Headers["Host"] = new Uri(convertedUrl).Host;

            var remoteRes = RemoteRequest(convertedUrl, remoteRequest, 60000);
            try
            {
                return JsonConvert.DeserializeObject<SearchResult>(Encoding.UTF8.GetString(remoteRes.Content));
            }catch(Exception e)
            {
                throw new Exception(Encoding.UTF8.GetString(remoteRes.Content));
            }
        }
    }
}
