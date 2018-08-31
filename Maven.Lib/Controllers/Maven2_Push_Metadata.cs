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
    public class Maven2_Push_Metadata : RestAPI
    {
        private IMavenArtifactsService _interfaceService;
        private readonly Guid repoId;
        private IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IRequestParser _requestParser;

        public Maven2_Push_Metadata(Guid repoId,
            IRepositoryEntitiesRepository repositoryEntitiesRepository, IRequestParser requestParser,
            IMavenArtifactsService interfaceService, params string[] paths)
            : base(null, paths)
        {
            _interfaceService = interfaceService;
            this.repoId = repoId;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._requestParser = requestParser;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest arg)
        {

            var idx = _requestParser.Parse(arg);
            var repo = _repositoryEntitiesRepository.GetById(repoId);
            var content = Encoding.UTF8.GetString(arg.Content);
            if (string.IsNullOrWhiteSpace(idx.Checksum))
            {
                _interfaceService.SetMetadataChecksums(repo.Id, idx, content);
            }
            else
            {

                _interfaceService.UpdateMetadata(repo.Id, idx, content);
            }

            return new SerializableResponse();
        }
    }
}
