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
        private readonly Guid repoId;
        private IIndexService _indexService;

        public V3_Index_Json(Guid repoId,IIndexService indexService, AppProperties properties, IRepositoryEntitiesRepository reps, params string[] paths) :
            base(properties, null, paths)
        {
            _reps = reps;
            this.repoId = repoId;
            _indexService = indexService;
            SetHandler(Handle);
        }

        public SerializableResponse Handle(SerializableRequest request)
        {
            var repo = _reps.GetById(repoId);
            var result = _indexService.Get(repo.Id);
            return this.JsonResponse(result);
        }
    }
}
