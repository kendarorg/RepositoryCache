using MultiRepositories.Repositories;
using MultiRepositories.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;
using System.IO;

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
            /*Assert.AreEqual("1", arg.PathParams["major"]);
            Assert.AreEqual("7", arg.PathParams["minor"]);
            Assert.AreEqual("25", arg.PathParams["patch"]);
            Assert.AreEqual("slf4j-api", arg.PathParams["package"]);
            Assert.AreEqual("org/slf4j", arg.PathParams["*path"]);
            Assert.AreEqual("slf4j-api-1.7.25", arg.PathParams["fullpackage"]);
            Assert.AreEqual("jar", arg.PathParams["type"]);
            Assert.AreEqual("md5", arg.PathParams["subtype"]);*/

            return new SerializableResponse();
        }
    }
}
