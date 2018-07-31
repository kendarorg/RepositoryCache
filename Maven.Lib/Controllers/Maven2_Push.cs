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
        private static int count = 0;
        private SerializableResponse Handler(SerializableRequest arg)
        {
            if (!arg.PathParams.ContainsKey("subtype"))
            {
                arg.PathParams["subtype"] = string.Empty;
            }
            /*var metadata = new MavenPackageMetadata
            {
                ArtifactId = arg.PathParams["package"],
                Group = arg.PathParams["*path"],
                SubType = arg.PathParams["subtype"]
            };*/

            /*Assert.AreEqual("slf4j-api", arg.PathParams["package"]);
            Assert.AreEqual("org/slf4j", arg.PathParams["*path"]);
            Assert.AreEqual("maven-metadata.xml", arg.PathParams["filename"]);
            Assert.AreEqual("asc", arg.PathParams["subtype"]);*/

            return new SerializableResponse();
        }
    }
}
