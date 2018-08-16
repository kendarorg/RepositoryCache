using MultiRepositories.Repositories;
using MultiRepositories.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;
using System.IO;
using MavenProtocol.Apis;
using Maven.Services;

namespace Maven.Controllers
{
    public class Maven2_Push_Package: RestAPI
    {
        private IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IRequestParser _requestParser;
        private readonly IMavenArtifactsService _mavenArtifactsService;

        public Maven2_Push_Package(
            IRepositoryEntitiesRepository repositoryEntitiesRepository, 
            IRequestParser requestParser,
            IMavenArtifactsService mavenArtifactsService,
            params string[] paths)
            : base(null, paths)
        {
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._requestParser = requestParser;
            this._mavenArtifactsService = mavenArtifactsService;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest arg)
        {
            var index = _requestParser.Parse(arg);
            var repo = _repositoryEntitiesRepository.GetByName(arg.PathParams["repo"]);
            if (!string.IsNullOrWhiteSpace(index.Checksum))
            {
                _mavenArtifactsService.WriteChecksum(repo.Id, index, Encoding.UTF8.GetString(arg.Content));
            }
            else
            {
                _mavenArtifactsService.WriteArtifact(repo.Id, index, arg.Content);
            }


            return new SerializableResponse();
        }
    }
}
