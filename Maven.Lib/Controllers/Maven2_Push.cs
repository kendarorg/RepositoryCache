using MultiRepositories.Repositories;
using MultiRepositories.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;

namespace Maven.Controllers
{
    public class Maven2_Push: RestAPI
    {
        private IRepositoryEntitiesRepository _repositoryEntitiesRepository;

        public Maven2_Push( IRepositoryEntitiesRepository repositoryEntitiesRepository, params string[] paths)
            : base(null, paths)
        {
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest arg)
        {
            return new SerializableResponse();
        }
    }
}
