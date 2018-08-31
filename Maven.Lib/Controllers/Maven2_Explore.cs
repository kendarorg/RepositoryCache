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
using MavenProtocol.Apis.Browse;

namespace Maven.Controllers
{
    public class Maven2_Explore : RestAPI
    {
        private readonly Guid repoId;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IRequestParser _requestParser;
        private readonly IMavenExploreService _mavenExploreService;

        public Maven2_Explore(Guid repoId,
            IRepositoryEntitiesRepository repositoryEntitiesRepository, IRequestParser requestParser, IMavenExploreService mavenExploreService, params string[] paths)
            : base(null, paths)
        {
            this.repoId = repoId;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._requestParser = requestParser;
            this._mavenExploreService = mavenExploreService;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest arg)
        {

            var idx = _requestParser.Parse(arg);
            var repo = _repositoryEntitiesRepository.GetById(repoId);
            var result = _mavenExploreService.Explore(repo.Id, idx);

            return new SerializableResponse();
        }
    }
}
