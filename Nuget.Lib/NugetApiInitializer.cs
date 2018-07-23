using Ioc;
using MultiRepositories;
using MultiRepositories.Repositories;
using Nuget.Controllers;
using NugetProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget
{
    public class NugetApiInitializer : IPackagesRepository
    {
        private IServicesMapper _servicesMapper;
        private ISearchQueryService _searchQueryService;
        private IIndexService _indexService;
        private AppProperties _applicationPropertes;
        private IRepositoryEntitiesRepository _availableRepositories;
        private List<IPackagesRepository> _packagesRepositories;
        private IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private IAssemblyUtils _assemblyUtils;

        public NugetApiInitializer(
            IRepositoryEntitiesRepository repositoryEntitiesRepository,
            AppProperties appProperties,
            IRepositoryEntitiesRepository availableRepositories,
            List<IPackagesRepository> packagesRepositories,
            IAssemblyUtils assemblyUtils,
            IIndexService indexService,
            ISearchQueryService searchQueryService,
            IServicesMapper servicesMapper
            )
        {
            _servicesMapper = servicesMapper;
            _searchQueryService = searchQueryService;
            _indexService = indexService;
            _applicationPropertes = appProperties;
            _availableRepositories = availableRepositories;
            _packagesRepositories = packagesRepositories;
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
            };
            repositoryServiceProvider.RegisterApi(new V3_Index_Json(_indexService,_applicationPropertes,_repositoryEntitiesRepository));

            repositoryServiceProvider.RegisterApi(new V3beta_Query("/{repo}/v3/query", _searchQueryService,_applicationPropertes,_repositoryEntitiesRepository, _servicesMapper));
            repositoryServiceProvider.RegisterApi(new V3beta_Query("/{repo}/v3rc/query", _searchQueryService, _applicationPropertes, _repositoryEntitiesRepository, _servicesMapper));
            repositoryServiceProvider.RegisterApi(new V3beta_Query("/{repo}/v3beta/query", _searchQueryService,_applicationPropertes,_repositoryEntitiesRepository, _servicesMapper));

            /*repositoryServiceProvider.RegisterApi(new V340_Registration_Package(pr, rp, ap, uc, reps));

            repositoryServiceProvider.RegisterApi(new V3_FlatContainer_Package_Version_Nupkg(ns, ap, uc, reps));
            repositoryServiceProvider.RegisterApi(new V3_FlatContainer_Package(ap, uc, reps));
            repositoryServiceProvider.RegisterApi(new V340_Registration_Package_Page(q, rp, ap, uc, reps));
            repositoryServiceProvider.RegisterApi(new V3_Registration_Package(rp, ap, uc, reps));
            repositoryServiceProvider.RegisterApi(new V3_Registration_Package_Version(pr, ap, uc, reps));
            repositoryServiceProvider.RegisterApi(new V3_Catalog_PackageId(pr, de, ap, uc, reps));*/
        }
    }
}
