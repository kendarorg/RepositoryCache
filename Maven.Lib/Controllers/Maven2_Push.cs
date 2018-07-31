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

namespace Maven.Controllers
{
    public class Maven2_Push : RestAPI
    {
        private IRepositoryEntitiesRepository _repositoryEntitiesRepository;

        public Maven2_Push(
            IRepositoryEntitiesRepository repositoryEntitiesRepository, params string[] paths)
            : base(null, paths)
        {
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest arg)
        {
            if (!arg.PathParams.ContainsKey("subtype"))
            {
                arg.PathParams["subtype"] = string.Empty;
            }

            var subType = arg.PathParams["subtype"];
            var mavenIndex = new MavenIndex
            {
                ArtifactId = arg.PathParams["package"],
                Checksum = arg.PathParams["subtype"],
                Group = arg.PathParams["*path"].Split('/'),
                Type = arg.PathParams["type"]
            };

            return new SerializableResponse();
        }
    }
}
