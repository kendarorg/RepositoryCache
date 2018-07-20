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
    public class NugetPackagePublishService : IPackagePublishService
    {
        private readonly IInsertNugetService _insertNugetService;

        public NugetPackagePublishService(IInsertNugetService insertNugetService)
        {
            this._insertNugetService = insertNugetService;
        }

        public void Create(Guid repoId, string nugetApiKey, byte[] data)
        {
            _insertNugetService.Insert(repoId, nugetApiKey, data);
        }

        public void Delist(Guid repoId, string nugetApiKey, string id, string version)
        {
            throw new NotImplementedException();
        }

        public void Relist(Guid repoId, string nugetApiKey, string id, string version)
        {
            throw new NotImplementedException();
        }
    }
}
