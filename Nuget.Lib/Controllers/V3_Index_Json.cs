//.Models;
//.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories.Repositories;
using MultiRepositories.Service;
using MultiRepositories;
using NugetProtocol;

namespace Nuget.Controllers
{
    public class V3_Index_Json : ForwardRestApi
    {
        private IRepositoryEntitiesRepository _reps;
        private IIndexService _indexService;

        public V3_Index_Json(IIndexService indexService, AppProperties properties, IRepositoryEntitiesRepository reps) :
            base(properties, "/{repo}/v3/index.json", null)
        {
            _reps = reps;
            _indexService = indexService;
            SetHandler(Handle);
        }

        public SerializableResponse Handle(SerializableRequest request)
        {
            var repo = _reps.GetByName(request.PathParams["repo"]);
            var result = _indexService.Get(repo.Id);
            return this.JsonResponse(result);
        }
    }
}
