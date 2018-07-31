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

namespace Maven.Controllers
{
    public class Maven2_Push_Package: RestAPI
    {
        private IRepositoryEntitiesRepository _repositoryEntitiesRepository;

        public Maven2_Push_Package(
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
            var version = "";
            if (arg.PathParams.ContainsKey("major")) version += arg.PathParams["major"];
            if (arg.PathParams.ContainsKey("minor")) version += "."+ arg.PathParams["major"];
            if (arg.PathParams.ContainsKey("patch")) version += "." + arg.PathParams["patch"];
            if (arg.PathParams.ContainsKey("extra")) version += "." + arg.PathParams["extra"];
            if (arg.PathParams.ContainsKey("pre")) version += "-" + arg.PathParams["pre"];


            
            var classifier = arg.PathParams["filename"].Replace(arg.PathParams["package"], "");
            if (string.IsNullOrWhiteSpace(classifier))
            {
                classifier = null;
            }
            var mavenIndex = new MavenIndex
            {
                ArtifactId= arg.PathParams["package"],
                Checksum = arg.PathParams["subtype"],
                Group= arg.PathParams["*path"].Split('/'),
                Version = version,
                Classifier = classifier
            };

            return new SerializableResponse();
        }
    }
}
