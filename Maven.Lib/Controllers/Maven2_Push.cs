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
        private SerializableResponse Handler(SerializableRequest localRequest)
        {
            count++;
            //repo
            //fullname
            //version
            //id
            //group
            var path = localRequest.ToLocalPath("index."+ count+".json");
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(path, JsonConvert.SerializeObject(localRequest));
            return new SerializableResponse();
        }
    }
}
