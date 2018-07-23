
using Ioc;
using MultiRepositories.Repositories;
using Nuget.Repositories;
using Nuget.Services;
using NugetProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Apis
{
    public class NugetBaseAddressService : IPackageBaseAddressService, ISingleton
    {
        private readonly IPackagesRepository _packagesRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IPackagesStorage _packagesStorage;

        public NugetBaseAddressService(
            IPackagesRepository packagesRepository,
            IRegistrationRepository registrationRepository,
            IRepositoryEntitiesRepository repositoryEntitiesRepository,
            IPackagesStorage packagesStorage)
        {
            _packagesRepository = packagesRepository;
            _registrationRepository = registrationRepository;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            _packagesStorage = packagesStorage;
        }

        public byte[] GetNupkg(Guid repoId, string lowerIdlowerVersion)
        {
            var repo = _repositoryEntitiesRepository.GetById(repoId);
            var package = _packagesRepository.GetByIdVersion(repoId, lowerIdlowerVersion);
            return _packagesStorage.Load(repo, package);
            
        }

        public string GetNuspec(Guid repoId, string lowerId, string lowerVersion)
        {
            var packageData = _packagesRepository.GetByPackage(repoId, lowerId, lowerVersion);
            return packageData.Nuspec;
        }

        public VersionsResult GetVersions(Guid repoId, string lowerId)
        {
            var versions = _registrationRepository.GetAllByPackageId(repoId, lowerId).
                Select(a => a.Version).ToList();
            return new VersionsResult(versions);
        }
    }
}
