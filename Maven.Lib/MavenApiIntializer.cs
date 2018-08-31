using Ioc;
using Maven.Controllers;
using Maven.Services;
using MavenProtocol;
using MavenProtocol.Apis;
using MavenProtocol.Apis.Browse;
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
        private readonly IMavenArtifactsService _artifactsService;
        private readonly IMavenExploreService _mavenExplorerService;

        public MavenApiIntializer(AppProperties applicationPropertes,
            IAssemblyUtils assemblyUtils,
            IRepositoryEntitiesRepository repositoryEntitiesRepository,
            IRequestParser requestParser,
            IMavenArtifactsService artifactsService,
            IMavenExploreService mavenExplorerService)
        {
            this._applicationPropertes = applicationPropertes;
            this._assemblyUtils = assemblyUtils;
            this._repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._requestParser = requestParser;
            _artifactsService = artifactsService;
            _mavenExplorerService = mavenExplorerService;
        }
        public void Initialize(IRepositoryServiceProvider repositoryServiceProvider)
        {
            var avail = _repositoryEntitiesRepository.GetAll().FirstOrDefault(a => a.Prefix == "repo.maven.apache.org");
            //https://repo.maven.apache.org/maven2/
            if (avail == null)
            {
                //var data = _assemblyUtils.ReadRes<MavenApiIntializer>("maven.org.settings.json");
                avail = new RepositoryEntity
                {
                    Mirror = true,
                    Prefix = "maven.apache",
                    Type = "maven",
                    Address = "/maven.apache",
                    PackagesPath = "maven.apache",
                    Settings = string.Empty
                };
                _repositoryEntitiesRepository.Save(avail);
            };

            var local = _repositoryEntitiesRepository.GetAll().FirstOrDefault(a => a.Prefix == "maven.local");

            if (local == null)
            {
                //var data = _assemblyUtils.ReadRes<MavenApiIntializer>("nuget.org.settings.json");
                local = new RepositoryEntity
                {
                    Mirror = false,
                    Prefix = "maven.local",
                    Type = "maven",
                    Address = "/maven.local",
                    PackagesPath = "maven.local",
                    Settings = string.Empty
                };
                _repositoryEntitiesRepository.Save(local);
            };

            foreach (var item in _repositoryEntitiesRepository.GetByType("maven"))
            {
                repositoryServiceProvider.RegisterApi(new Maven2_Explore(item.Id,_repositoryEntitiesRepository, _requestParser, _mavenExplorerService,
                    "*GET",
                        MavenConstants.REGEX_ONLY_PACK.
                            Replace("{repo}", Regex.Escape(item.Prefix)),
                    "*GET",
                        MavenConstants.REGEX_ONLY_META.
                            Replace("{repo}", Regex.Escape(item.Prefix)),
                    "*GET",
                        @"/{repo}/{*path}/" + ///maven.local/org/slf4j
                        @"{pack#" + MavenConstants.PACKAGE_REGEXP + @"}/" + //slf4j-api/
                        @"{version#" + MavenConstants.VERSION_REGEXP + @"}". //1.7.2
                            Replace("{repo}", item.Prefix),
                    "*GET",
                        @"/{repo}/{*path}/" +//maven.local/org/slf4j/
                        @"{pack#" + MavenConstants.PACKAGE_REGEXP + @"}".//slf4j-api/
                            Replace("{repo}", item.Prefix),
                    "*GET",
                        @"/{repo}/{*path}".
                            Replace("{repo}", item.Prefix)//maven.local/org/slf4j/
                    ));

                repositoryServiceProvider.RegisterApi(
                    new Maven2_Push_Package(item.Id, _repositoryEntitiesRepository, _requestParser, _artifactsService,
                    "*PUT",
                        MavenConstants.REGEX_ONLY_PACK.
                            Replace("{repo}", Regex.Escape(item.Prefix))));

                repositoryServiceProvider.RegisterApi(
                    new Maven2_Push_Metadata(item.Id, _repositoryEntitiesRepository, _requestParser, _artifactsService,
                    "*PUT",
                        MavenConstants.REGEX_ONLY_META.
                            Replace("{repo}", Regex.Escape(item.Prefix))));
            }
        }
    }
}
