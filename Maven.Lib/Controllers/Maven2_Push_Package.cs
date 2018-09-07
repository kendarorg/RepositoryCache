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
using Maven.News;
using MavenProtocol.News;

namespace Maven.Controllers
{
    public class Maven2_Push_Package : RestAPI
    {
        private readonly IArtifactsApi _interfaceService;
        private readonly IPomApi _pomApi;
        private readonly Guid _repoId;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IRequestParser _requestParser;

        public Maven2_Push_Package(Guid repoId,
            IRepositoryEntitiesRepository repositoryEntitiesRepository, IRequestParser requestParser,
            IArtifactsApi interfaceService, IPomApi pomApi, params string[] paths)
            : base(null, paths)
        {
            _interfaceService = interfaceService;
            this._pomApi = pomApi;
            this._repoId = repoId;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._requestParser = requestParser;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest arg)
        {

            var idx = _requestParser.Parse(arg);
            idx.RepoId = _repoId;

            if (_interfaceService.CanHandle(idx))
            {
                _interfaceService.Generate(idx, false);
            }
            else if (_pomApi.CanHandle(idx))
            {
                _pomApi.Generate(idx, false);
            }

            return new SerializableResponse();
        }
    }
}
