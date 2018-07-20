
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
    public class NugetBaseAddressService : IPackageBaseAddressService,ISingleton
    {
        private readonly IInsertNugetService _insertNugetService;

        public NugetBaseAddressService(IInsertNugetService insertNugetService)
        {
            this._insertNugetService = insertNugetService;
        }

        public byte[] GetNupkg(Guid repoId, string lowerIdlowerVersion)
        {
            //_insertNugetService.Insert(repoId, nugetApiKey, data);
            throw new NotImplementedException();
        }

        public string GetNuspec(Guid repoId, string lowerId, string lowerVersion)
        {
            throw new NotImplementedException();
        }

        public VersionsResult GetVersions(Guid repoId, string lowerId)
        {
            throw new NotImplementedException();
        }
    }
}
