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
        {/*
            http://localhost:9088/maven.local/org/slf4j/slf4j-api/1.7.25/slf4j-api-1.7.25.jar
http://localhost:9088/maven.local/org/slf4j/slf4j-api/1.7.25/slf4j-api-1.7.25.jar.sha1
http://localhost:9088/maven.local/org/slf4j/slf4j-api/1.7.25/slf4j-api-1.7.25.jar.md5
http://localhost:9088/maven.local/org/slf4j/slf4j-api/1.7.25/slf4j-api-1.7.25.pom
http://localhost:9088/maven.local/org/slf4j/slf4j-api/1.7.25/slf4j-api-1.7.25.pom.sha1
http://localhost:9088/maven.local/org/slf4j/slf4j-api/1.7.25/slf4j-api-1.7.25.pom.md5
http://localhost:9088/maven.local/org/slf4j/slf4j-api/maven-metadata.xml
http://localhost:9088/maven.local/org/slf4j/slf4j-api/maven-metadata.xml
http://localhost:9088/maven.local/org/slf4j/slf4j-api/maven-metadata.xml.sha1
http://localhost:9088/maven.local/org/slf4j/slf4j-api/maven-metadata.xml.md5
*/
            //repo
            //fullname
            //version
            //id
            //group
            //File.WriteAllText(arg.PathParams["fullname"],JsonConvert.)
            return new SerializableResponse();
        }
    }
}
