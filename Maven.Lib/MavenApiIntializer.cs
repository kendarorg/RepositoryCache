using Ioc;
using Maven.Controllers;
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

            var allowedPackageApis = new List<string>();

            foreach (var item in new string[] { "jar", "pom" })
            {
                var toAdd = "/{repo}/{*group}/{id}/{version}/{fullName}." + item;
                allowedPackageApis.Add("*PUT");
                allowedPackageApis.Add("*GET");
                allowedPackageApis.Add(toAdd);
                foreach (var sub in new string[] { "asc", "md5", "sha1" })
                {
                    allowedPackageApis.Add("*PUT");
                    allowedPackageApis.Add("*GET");
                    allowedPackageApis.Add(toAdd + "." + sub);
                }
            }

            repositoryServiceProvider.RegisterApi(new Maven2_Push_Package(_repositoryEntitiesRepository,
               allowedPackageApis.ToArray()));

            var allowedPackageGroupApis = new List<string>();
            var add = "/{repo}/{*group}/{id}/maven-metadata.xml";
            allowedPackageGroupApis.Add("*PUT");
            allowedPackageGroupApis.Add("*GET");
            allowedPackageGroupApis.Add(add);
            foreach (var sub in new string[] { "asc", "md5", "sha1" })
            {
                allowedPackageGroupApis.Add("*PUT");
                allowedPackageGroupApis.Add("*GET");
                allowedPackageGroupApis.Add(add + "." + sub);
            }
            repositoryServiceProvider.RegisterApi(new Maven2_Push(_repositoryEntitiesRepository,
              allowedPackageGroupApis.ToArray()));
        }
    }
}
