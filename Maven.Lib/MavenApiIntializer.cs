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

            repositoryServiceProvider.RegisterApi(new Maven2_Push_Package(_repositoryEntitiesRepository,
              "*GET", "*PUT", @"/{repo}/{*path}/" +
                @"{pack#^(?<package>[0-9A-Za-z\-\.]+)$}/" +
                @"{ver#^(?<major>\d+)" +
                @"(\.(?<minor>\d+))?" +
                @"(\.(?<patch>\d+))?" +
                @"(\.(?<extra>\d+))?" +
                @"(\-(?<pre>[0-9A-Za-z\.]+))?" +
                @"(\-(?<build>[0-9A-Za-z\-\.]+))?$}/" +
                @"{filename#^(?<fullpackage>(?:(?!\b(?:jar|pom)\b)[0-9A-Za-z\-\.])+)?" +
                @"\.(?<type>(jar|pom))" +
                @"(\.(?<subtype>(asc|md5|sha1)))?$}"));

            repositoryServiceProvider.RegisterApi(new Maven2_Push(_repositoryEntitiesRepository,
              "*GET", "*PUT", @"/{repo}/{*path}/" +
                @"{pack#^(?<package>[0-9A-Za-z\-\.]+)$}/" +
                @"{meta#^(?<filename>(maven-metadata.xml))(\.(?<subtype>(asc|md5|sha1)))?$}"));
        }
    }
}
