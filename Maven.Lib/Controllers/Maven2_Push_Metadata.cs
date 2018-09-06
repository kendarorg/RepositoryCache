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
    public class Maven2_Push_Metadata : RestAPI
    {
        private IMetadataApi _interfaceService;
        private readonly Guid _repoId;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IRequestParser _requestParser;

        public Maven2_Push_Metadata(Guid repoId,
            IRepositoryEntitiesRepository repositoryEntitiesRepository, IRequestParser requestParser,
            IMetadataApi interfaceService, params string[] paths)
            : base(null, paths)
        {
            _interfaceService = interfaceService;
            this._repoId = repoId;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._requestParser = requestParser;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest arg)
        {

            var idx = _requestParser.Parse(arg);
            idx.RepoId = _repoId;
            _interfaceService.Generate(idx);

            return new SerializableResponse();
        }
    }
}
