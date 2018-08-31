using MultiRepositories.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;
using Nuget.Services;
using MultiRepositories.Repositories;
using System.IO;

namespace Nuget.Controllers
{
    public class Custom_Load:RestAPI
    {
        private IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly Guid repoId;
        private IInsertNugetService _insertNugetService;

        public Custom_Load(Guid repoId,IInsertNugetService  insertNugetService, IRepositoryEntitiesRepository repositoryEntitiesRepository, params string[] paths)
            : base(null, paths)
        {
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            this.repoId = repoId;
            _insertNugetService = insertNugetService;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest localRequest)
        {
            var result = new List<string>();
            var repo = _repositoryEntitiesRepository.GetById(repoId);
            var dir = localRequest.QueryParams["dir"];
            if (!Directory.Exists(dir))
            {
                throw new Exception();
            }
            foreach(var file in Directory.GetFiles(dir, "*.nupkg"))
            {
                try
                {
                    _insertNugetService.Insert(repo.Id, null, File.ReadAllBytes(file));
                    result.Add("Loaded " + Path.GetFileNameWithoutExtension(file));
                }
                catch
                {
                    result.Add("Error " + Path.GetFileNameWithoutExtension(file));
                }
            }
            return JsonResponse(result);
        }
        
    }
}
