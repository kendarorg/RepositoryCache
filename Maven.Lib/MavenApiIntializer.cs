using Ioc;
using Maven.Controllers;
using MavenProtocol;
using MultiRepositories;
using MultiRepositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven
{
    public class MavenApiIntializer : IApiProvider
    {
        private readonly AppProperties _applicationPropertes;
        private readonly IAssemblyUtils _assemblyUtils;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;

        public MavenApiIntializer(AppProperties applicationPropertes,
            IAssemblyUtils assemblyUtils,
            IRepositoryEntitiesRepository repositoryEntitiesRepository)
        {
            this._applicationPropertes = applicationPropertes;
            this._assemblyUtils = assemblyUtils;
            this._repositoryEntitiesRepository = repositoryEntitiesRepository;
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

            foreach (var item in _repositoryEntitiesRepository.GetByType("nuget"))
            {

                repositoryServiceProvider.RegisterApi(new Maven2_Push_Package(_repositoryEntitiesRepository,
                    "*GET", "*PUT",
                    @"/{repo}/{*path}/" + ///maven.local/org/slf4j
                    @"{pack#" + MavenConstants.PACKAGE_REGEXP + @"}/" + //slf4j-api/
                    @"{ver#" + MavenConstants.VERSION_REGEXP + @"}/" +
                    @"{meta#" + MavenConstants.FULLPACKAGE_AND_CHECHKSUMS_REGEXP + @"}".
                        Replace("{repo}", item.Prefix))); //slf4j-api-1.7.25.jar.md5

                repositoryServiceProvider.RegisterApi(new Maven2_Push(_repositoryEntitiesRepository,
                    "*GET", "*PUT",
                    @"/{repo}/{*path}/" +//maven.local/org/slf4j/
                    @"{pack#" + MavenConstants.PACKAGE_REGEXP + @"}/" + //slf4j-api/
                    @"{meta#" + MavenConstants.METADATA_AND_CHECHKSUMS_REGEXP + @"}".
                        Replace("{repo}", item.Prefix))); //maven-metadata.xml.asc
            }
        }
    }
}
