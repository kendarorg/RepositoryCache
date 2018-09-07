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

namespace Maven.Controllers
{
    public class Maven2_Search : RestAPI
    {
        private IMetadataApi _interfaceService;
        private readonly IMavenSearch _mavenSearch;
        private readonly Guid _repoId;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IRequestParser _requestParser;

        public Maven2_Search(Guid repoId,
            IRepositoryEntitiesRepository repositoryEntitiesRepository, IRequestParser requestParser,
            IMetadataApi interfaceService, IMavenSearch mavenSearch, params string[] paths)
            : base(null, paths)
        {
            _interfaceService = interfaceService;
            this._mavenSearch = mavenSearch;
            this._repoId = repoId;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._requestParser = requestParser;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest arg)
        {
            var idx = _requestParser.Parse(arg);
            idx.RepoId = _repoId;
            var sp = new SearchParam();
            if (arg.QueryParams.ContainsKey("q")) sp.Query = arg.QueryParams["q"];
            if (arg.QueryParams.ContainsKey("rows")) sp.Rows = int.Parse(arg.QueryParams["rows"]);
            if (arg.QueryParams.ContainsKey("skip")) sp.Rows = int.Parse(arg.QueryParams["skip"]);
            if (arg.QueryParams.ContainsKey("core")) sp.Core = arg.QueryParams["core"];
            if (arg.QueryParams.ContainsKey("wt")) sp.Core = arg.QueryParams["wt"];
            var result = _mavenSearch.Search(_repoId, sp);

            return JsonResponse(result);
        }
    }
}
