using Ioc;
using Nuget.Repositories;
using Nuget.Services;
using NugetProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Apis
{
    public class NugetPackagePublishService : IPackagePublishService, ISingleton
    {
        private readonly IQueryRepository _queryRepository;
        private readonly IInsertNugetService _insertNugetService;

        public NugetPackagePublishService(
            IInsertNugetService insertNugetService,
            IQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
            this._insertNugetService = insertNugetService;
        }

        public void Create(Guid repoId, string nugetApiKey, byte[] data)
        {
            nugetApiKey = nugetApiKey.ToUpperInvariant();
            _insertNugetService.Insert(repoId, nugetApiKey, data);
        }

        public void Delist(Guid repoId, string nugetApiKey, string id, string version)
        {
            nugetApiKey = nugetApiKey.ToUpperInvariant();
            id = id.ToLowerInvariant();
            version = version.ToLowerInvariant();
            var query = _queryRepository.GetByPackage(repoId, id, version);
            if (query != null)
            {
                query.Listed = false;
                _queryRepository.Update(query);
            }
        }

        public void Relist(Guid repoId, string nugetApiKey, string id, string version)
        {
            nugetApiKey = nugetApiKey.ToUpperInvariant();
            id = id.ToLowerInvariant();
            version = version.ToLowerInvariant();
            var query = _queryRepository.GetByPackage(repoId, id, version);
            if (query != null)
            {
                query.Listed = true;
                _queryRepository.Update(query);
            }
        }
    }
}
