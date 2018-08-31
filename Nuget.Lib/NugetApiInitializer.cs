using Ioc;
using MultiRepositories;
using MultiRepositories.Repositories;
using Nuget.Controllers;
using Nuget.Repositories;
using Nuget.Services;
using NugetProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget
{
    public class NugetApiInitializer : IApiProvider
    {
        private readonly ICatalogService _catalogService;
        private readonly IPackagesRepository _repositoryOfPackages;
        private readonly IPackageBaseAddressService _packageBaseAddressService;
        private readonly IInsertNugetService _insertNugetService;
        private readonly IRegistrationService _registrationService;
        private readonly IServicesMapper _servicesMapper;
        private readonly ISearchQueryService _searchQueryService;
        private readonly IIndexService _indexService;
        private readonly AppProperties _applicationPropertes;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IAssemblyUtils _assemblyUtils;

        public NugetApiInitializer(
            ICatalogService catalogService,
            IRepositoryEntitiesRepository repositoryEntitiesRepository,
            AppProperties appProperties,
            IRepositoryEntitiesRepository availableRepositories,
            IAssemblyUtils assemblyUtils,
            IIndexService indexService,
            ISearchQueryService searchQueryService,
            IServicesMapper servicesMapper,
            IRegistrationService registrationService,
            IInsertNugetService insertNugetService,
            IPackageBaseAddressService packageBaseAddressService,
            IPackagesRepository repositoryOfPackages
            )
        {
            _catalogService = catalogService;
            _repositoryOfPackages = repositoryOfPackages;
            _packageBaseAddressService = packageBaseAddressService;
            _insertNugetService = insertNugetService;
            _registrationService = registrationService;
            _servicesMapper = servicesMapper;
            _searchQueryService = searchQueryService;
            _indexService = indexService;
            _applicationPropertes = appProperties;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            _assemblyUtils = assemblyUtils;
        }

        public void Initialize(IRepositoryServiceProvider repositoryServiceProvider)
        {
            var avail = _repositoryEntitiesRepository.GetAll().FirstOrDefault(a => a.Prefix == "nuget.org");

            if (avail == null)
            {
                var data = _assemblyUtils.ReadRes<NugetApiInitializer>("nuget.org.settings.json");
                avail = new RepositoryEntity
                {
                    Mirror = true,
                    Prefix = "nuget.org",
                    Type = "nuget",
                    Address = "nuget.org/v3/index.json",
                    PackagesPath = "nuget.org",
                    Settings = data
                };
                _repositoryEntitiesRepository.Save(avail);
            };

            var local = _repositoryEntitiesRepository.GetAll().FirstOrDefault(a => a.Prefix == "nuget.local");

            if (local == null)
            {
                var data = _assemblyUtils.ReadRes<NugetApiInitializer>("nuget.org.settings.json");
                local = new RepositoryEntity
                {
                    Mirror = false,
                    Prefix = "nuget.local",
                    Type = "nuget",
                    Address = "/nuget.local/v3/index.json",
                    PackagesPath = "nuget.local",
                    Settings = data
                };
                _repositoryEntitiesRepository.Save(local);
            };

            _servicesMapper.Refresh();

            foreach (var item in _repositoryEntitiesRepository.GetByType("nuget"))
            {

                repositoryServiceProvider.RegisterApi(new V2_Publish(item.Id,_insertNugetService, _repositoryEntitiesRepository,
                    "/{repo}/v2/publish".Replace("{repo}",item.Prefix)));

                repositoryServiceProvider.RegisterApi(new V3_Index_Json(item.Id,
                    _indexService, _applicationPropertes, _repositoryEntitiesRepository,
                    "/{repo}/v3/index.json".Replace("{repo}", item.Prefix)));

                repositoryServiceProvider.RegisterApi(new V3_Query(item.Id,
                    _searchQueryService, _applicationPropertes, _repositoryEntitiesRepository, _servicesMapper,
                    "/{repo}/v3/query".Replace("{repo}", item.Prefix)));

                repositoryServiceProvider.RegisterApi(new V3_Registration_Package(item.Id,
                    _applicationPropertes, _repositoryEntitiesRepository, _registrationService, _servicesMapper,
                    "/{repo}/v3/registration/{semver}/{packageid}/index.json".Replace("{repo}", item.Prefix)));

                repositoryServiceProvider.RegisterApi(new V3_Registration_Package_Version(item.Id,
                    _applicationPropertes, _repositoryEntitiesRepository, _registrationService, _servicesMapper,
                    "/{repo}/v3/registration/{semver}/{packageid}/{version}.json".Replace("{repo}", item.Prefix)));

                repositoryServiceProvider.RegisterApi(new V3_Registration_Package_Page(item.Id,
                    _applicationPropertes, _registrationService, _servicesMapper, _repositoryEntitiesRepository,
                    "/{repo}/v3/registration/{semver}/{packageid}/page/{from}/{to}.json".Replace("{repo}", item.Prefix)));

                repositoryServiceProvider.RegisterApi(new V3_FlatContainer_Package_Version_Nupkg(item.Id,
                    _applicationPropertes, _insertNugetService, _repositoryOfPackages,
                    _packageBaseAddressService, _servicesMapper, _repositoryEntitiesRepository,
                    "/{repo}/v3/container/{idLower}/{versionLower}/{fullversion}.nupkg".Replace("{repo}", item.Prefix)));

                repositoryServiceProvider.RegisterApi(new V3_FlatContainer_Package(item.Id,
                    _applicationPropertes, _servicesMapper, _repositoryEntitiesRepository, _packageBaseAddressService,
                    "/{repo}/v3/container/{packageid}/index.json".Replace("{repo}", item.Prefix)));


                repositoryServiceProvider.RegisterApi(new V3_Catalog_PackageId(item.Id,
                    _applicationPropertes, _catalogService, _repositoryEntitiesRepository, _registrationService, _servicesMapper,
                    "/{repo}/v3/catalog/data/{date}/{fullPackage}.json".Replace("{repo}", item.Prefix)));

                repositoryServiceProvider.RegisterApi(new Custom_Load(item.Id, _insertNugetService, _repositoryEntitiesRepository,
                    "/{repo}/custom/load".Replace("{repo}", item.Prefix)));
            }
            /*repositoryServiceProvider.RegisterApi(new V3_Catalog_PackageId(pr, de, ap, uc, reps));*/
        }
    }
}
