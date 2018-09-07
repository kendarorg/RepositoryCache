using Ioc;
using Maven.Controllers;
using Maven.News;
using Maven.Services;
using MavenProtocol;
using MavenProtocol.Apis;
using MavenProtocol.Apis.Browse;
using MavenProtocol.News;
using MultiRepositories;
using MultiRepositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Maven
{
    public class MavenApiIntializer : IApiProvider
    {
        private readonly AppProperties _applicationPropertes;
        private readonly IAssemblyUtils _assemblyUtils;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IRequestParser _requestParser;
        private readonly IServicesMapper _servicesMapper;
        private readonly IExploreApi _exploreApi;
        private readonly IPomApi _pomApi;
        private readonly IArtifactsApi _artifactsApi;
        private readonly IMetadataApi _metadataApi;
        private readonly IMetadataRepository _metadataRepository;

        public MavenApiIntializer(AppProperties applicationPropertes,
            IAssemblyUtils assemblyUtils,
            IRepositoryEntitiesRepository repositoryEntitiesRepository,
            IRequestParser requestParser,
            IServicesMapper servicesMapper, IExploreApi exploreApi, IPomApi pomApi, IArtifactsApi artifactsApi, IMetadataApi metadataApi,
            IMetadataRepository metadataRepository)
        {
            this._applicationPropertes = applicationPropertes;
            this._assemblyUtils = assemblyUtils;
            this._repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._requestParser = requestParser;
            this._servicesMapper = servicesMapper;
            this._exploreApi = exploreApi;
            this._pomApi = pomApi;
            this._artifactsApi = artifactsApi;
            this._metadataApi = metadataApi;
            this._metadataRepository = metadataRepository;
        }
        public void Initialize(IRepositoryServiceProvider repositoryServiceProvider)
        {
            var avail = _repositoryEntitiesRepository.GetAll().FirstOrDefault(a => a.Prefix == "maven.apache");
            //https://repo.maven.apache.org/maven2/
            if (avail == null)
            {
                var data = _assemblyUtils.ReadRes<MavenApiIntializer>("maven.org.settings.json");
                avail = new RepositoryEntity
                {
                    Mirror = true,
                    Prefix = "maven.apache",
                    Type = "maven",
                    Address = "/maven.apache",
                    PackagesPath = "maven.apache",
                    Settings = data
                };
                _repositoryEntitiesRepository.Save(avail);
            };

            var local = _repositoryEntitiesRepository.GetAll().FirstOrDefault(a => a.Prefix == "maven.local");

            if (local == null)
            {
                var data = _assemblyUtils.ReadRes<MavenApiIntializer>("maven.org.settings.json");
                local = new RepositoryEntity
                {
                    Mirror = false,
                    Prefix = "maven.local",
                    Type = "maven",
                    Address = "/maven.local",
                    PackagesPath = "maven.local",
                    Settings = data
                };
                _repositoryEntitiesRepository.Save(local);
            };
            _servicesMapper.Refresh();
            foreach (var item in _repositoryEntitiesRepository.GetByType("maven"))
            {
                repositoryServiceProvider.RegisterApi(new Maven2_Explore(
                    item.Id, _applicationPropertes, _repositoryEntitiesRepository, _servicesMapper, _requestParser,
                    _exploreApi, _pomApi, _artifactsApi, _metadataApi, _metadataRepository,
                    "*GET",
                        MavenConstants.REGEX_SNAP_PACK.
                            Replace("{repo}", Regex.Escape(item.Prefix)),
                    "*GET",
                        MavenConstants.REGEX_SNAP_PACK_CHECK.
                            Replace("{repo}", Regex.Escape(item.Prefix)),
                    "*GET",
                        MavenConstants.REGEX_SNAP_META.
                            Replace("{repo}", Regex.Escape(item.Prefix)),

                    "*GET",
                        MavenConstants.REGEX_ONLY_PACK.
                            Replace("{repo}", Regex.Escape(item.Prefix)),
                    "*GET",
                        MavenConstants.REGEX_ONLY_META.
                            Replace("{repo}", Regex.Escape(item.Prefix)),
                    "*GET",
                        (@"/{repo}/{*path}/" + ///maven.local/org/slf4j
                        @"{pack#" + MavenConstants.PACKAGE_REGEXP + @"}/" + //slf4j-api/
                        @"{version#" + MavenConstants.VERSION_REGEXP + @"}"). //1.7.2
                            Replace("{repo}", item.Prefix),
                    "*GET",
                        @"/{repo}/{*path}".
                            Replace("{repo}", item.Prefix),//maven.local/org/slf4j/

                    "*GET",
                        item.Prefix
                    ));

                repositoryServiceProvider.RegisterApi(
                    new Maven2_Push_Package(item.Id, _repositoryEntitiesRepository, _requestParser, _artifactsApi, _pomApi,
                    "*PUT",
                        MavenConstants.REGEX_SNAP_PACK.
                            Replace("{repo}", Regex.Escape(item.Prefix)),
                    "*PUT",
                        MavenConstants.REGEX_SNAP_PACK_CHECK.
                            Replace("{repo}", Regex.Escape(item.Prefix)),
                    "*PUT",
                        MavenConstants.REGEX_ONLY_PACK.
                            Replace("{repo}", Regex.Escape(item.Prefix))));

                repositoryServiceProvider.RegisterApi(
                    new Maven2_Push_Metadata(item.Id, _repositoryEntitiesRepository, _requestParser, _metadataApi,
                    "*PUT",
                        MavenConstants.REGEX_SNAP_META.
                            Replace("{repo}", Regex.Escape(item.Prefix)),
                    "*PUT",
                        MavenConstants.REGEX_ONLY_META.
                            Replace("{repo}", Regex.Escape(item.Prefix))));
            }
        }
    }
}
