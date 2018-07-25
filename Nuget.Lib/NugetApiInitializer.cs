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
        private ICatalogService _catalogService;
        private IPackagesRepository _repositoryOfPackages;
        private IPackageBaseAddressService _packageBaseAddressService;
        private IInsertNugetService _insertNugetService;
        private IRegistrationService _registrationService;
        private IServicesMapper _servicesMapper;
        private ISearchQueryService _searchQueryService;
        private IIndexService _indexService;
        private AppProperties _applicationPropertes;
        private IRepositoryEntitiesRepository _availableRepositories;
        private IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private IAssemblyUtils _assemblyUtils;

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
            _availableRepositories = availableRepositories;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            _assemblyUtils = assemblyUtils;
        }

        public void Initialize(IRepositoryServiceProvider repositoryServiceProvider)
        {
            var avail = _repositoryEntitiesRepository.GetAll().FirstOrDefault(a => a.Address == "http://api.nuget.org");

            if (avail == null)
            {
                var data = _assemblyUtils.ReadRes<NugetApiInitializer>("nuget.org.settings.json");
                avail = new RepositoryEntity
                {
                    Mirror = true,
                    Prefix = "nuget.org",
                    Type = "nuget",
                    Address = "http://api.nuget.org",
                    PackagesPath = "nuget.org",
                    Settings = data
                };
                _repositoryEntitiesRepository.Save(avail);
                _servicesMapper.Refresh();
            };
            repositoryServiceProvider.RegisterApi(new V3_Index_Json(
                _indexService, _applicationPropertes, _repositoryEntitiesRepository,
                "/{repo}/v3/index.json"));

            repositoryServiceProvider.RegisterApi(new V3_Query(
                _searchQueryService, _applicationPropertes, _repositoryEntitiesRepository, _servicesMapper,
                "/{repo}/v3/query"));

            repositoryServiceProvider.RegisterApi(new V3_Registration_Package(
                _applicationPropertes, _repositoryEntitiesRepository, _registrationService, _servicesMapper,
                "/{repo}/v3/registration/{semver}/{packageid}/index.json"));

            repositoryServiceProvider.RegisterApi(new V3_Registration_Package_Version(
                _applicationPropertes, _repositoryEntitiesRepository, _registrationService, _servicesMapper,
                "/{repo}/v3/registration/{semver}/{packageid}/{version}.json"));

            repositoryServiceProvider.RegisterApi(new V3_Registration_Package_Page(
                _applicationPropertes, _registrationService, _servicesMapper, _repositoryEntitiesRepository,
                "/{repo}/v3/registration/{semver}/{packageid}/page/{from}/{to}.json"));

            repositoryServiceProvider.RegisterApi(new V3_FlatContainer_Package_Version_Nupkg(
                _applicationPropertes, _insertNugetService, _repositoryOfPackages,
                _packageBaseAddressService, _servicesMapper, _repositoryEntitiesRepository,
                "/{repo}/v3/container/{idLower}/{versionLower}/{fullversion}.nupkg"));

            repositoryServiceProvider.RegisterApi(new V3_FlatContainer_Package(
                _applicationPropertes, _servicesMapper, _repositoryEntitiesRepository, _packageBaseAddressService,
                "/{repo}/v3/container/{packageid}/index.json"));


            repositoryServiceProvider.RegisterApi(new V3_Catalog_PackageId(
                _applicationPropertes, _catalogService, _repositoryEntitiesRepository, _registrationService, _servicesMapper,
                "/{repo}/v3/catalog/data/{date}/{fullPackage}.json"));

            repositoryServiceProvider.RegisterApi(new Custom_Load(_insertNugetService, _repositoryEntitiesRepository,
                "/{repo}/custom/load"));
            /*repositoryServiceProvider.RegisterApi(new V3_Catalog_PackageId(pr, de, ap, uc, reps));*/
        }
    }
}
