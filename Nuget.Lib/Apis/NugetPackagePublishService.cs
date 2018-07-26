using Ioc;
using MultiRepositories.Repositories;
using Nuget.Repositories;
using Nuget.Services;
using NugetProtocol;
using Repositories;
using SemVer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Apis
{
    public class NugetPackagePublishService : IPackagePublishService, ISingleton
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IQueryRepository _queryRepository;
        private readonly IInsertNugetService _insertNugetService;

        public NugetPackagePublishService(
            IInsertNugetService insertNugetService,
            IQueryRepository queryRepository,
            IRepositoryEntitiesRepository repositoryEntitiesRepository,
            IRegistrationRepository registrationRepository,
            ITransactionManager transactionManager)
        {
            _registrationRepository = registrationRepository;
            _transactionManager = transactionManager;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
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
            ChangeListedStatus(repoId, id, version, false);
        }

        private void ChangeListedStatus(Guid repoId, string id, string version, bool listed)
        {
            var semVersion = SemVersion.Parse(version);
            var isPre = !string.IsNullOrWhiteSpace(semVersion.Prerelease);

            using (var transaction = _transactionManager.BeginTransaction())
            {
                try
                {
                    var registrationEntities = _registrationRepository.GetAllByPackageId(repoId, id)
                        .OrderByDescending((a) => SemVersion.Parse(a.Version));
                    var registrationEntity = registrationEntities.FirstOrDefault(a => a.Version == version);
                    if (registrationEntity.Listed == listed)
                    {
                        return;
                    }
                    registrationEntity.Listed = listed;
                    var firstPreListed = registrationEntities.FirstOrDefault(a => a.Listed && isPre == string.IsNullOrWhiteSpace(a.PreRelease));
                    var firstOffListed = registrationEntities.FirstOrDefault(a => a.Listed && isPre != string.IsNullOrWhiteSpace(a.PreRelease));

                    var packageEntity = _queryRepository.GetByPackage(repoId, id);

                    packageEntity.PreListed = firstPreListed != null;
                    packageEntity.PreVersion = firstPreListed.Version;
                    packageEntity.Listed = firstOffListed != null;
                    packageEntity.Version = firstOffListed.Version;
                    _queryRepository.Update(packageEntity, transaction);
                    _registrationRepository.Update(registrationEntity, transaction);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
        }

        public void Relist(Guid repoId, string nugetApiKey, string id, string version)
        {
            nugetApiKey = nugetApiKey.ToUpperInvariant();
            id = id.ToLowerInvariant();
            version = version.ToLowerInvariant();
            ChangeListedStatus(repoId, id, version, true);
        }
    }
}
