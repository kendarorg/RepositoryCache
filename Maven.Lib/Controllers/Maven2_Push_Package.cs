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

namespace Maven.Controllers
{
    public class Maven2_Push_Package : RestAPI
    {
        private IArtifactsService _interfaceService;
        private IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IRequestParser _requestParser;

        public Maven2_Push_Package(
            IRepositoryEntitiesRepository repositoryEntitiesRepository, IRequestParser requestParser,
            IArtifactsService interfaceService, params string[] paths)
            : base(null, paths)
        {
            _interfaceService = interfaceService;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._requestParser = requestParser;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest arg)
        {

            var idx = _requestParser.Parse(arg);
            var repo = _repositoryEntitiesRepository.GetByName(arg.PathParams["repoId"]);

            if (string.IsNullOrWhiteSpace(idx.Checksum))
            {
                var content = Encoding.UTF8.GetString(arg.Content);
                _interfaceService.SetArtifactChecksums(repo.Id, idx, content);
            }
            else
            {
                _interfaceService.UploadArtifact(repo.Id, idx, arg.Content);
            }
            return new SerializableResponse();
        }
        /*
        private void HandleChecksums(MavenIndex idx, SerializableRequest request)
        {
            if ("pom" == idx.Type)
            {
                //Handle pom
                _interfaceService.SetMainArtifactPomChecksum(idx, request.Content,idx.Checksum);
            }
            else if (string.IsNullOrWhiteSpace(idx.Classifier))
            {
                _interfaceService.SetMainArtifactChecksum(idx, request.Content, idx.Checksum);
            }
            else
            {
                _interfaceService.SetClassifierArtifactChecksum(idx, request.Content, idx.Checksum);
            }
        }

        private void HandleRealArtifacts(MavenIndex idx, SerializableRequest request)
        {
            if ("pom" == idx.Type)
            {
                //Handle pom
                _interfaceService.UploadMainArtifactPom(idx, request.Content);
            }
            else if (string.IsNullOrWhiteSpace(idx.Classifier))
            {
                _interfaceService.UploadMainArtifact(idx, request.Content);
            }
            else
            {
                _interfaceService.UploadClassifierArtifact(idx, request.Content);
            }
        }*/
    }
}
