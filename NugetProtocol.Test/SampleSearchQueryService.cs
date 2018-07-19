using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class SampleSearchQueryService : ISearchQueryService
    {
        private IServicesMapper _servicesMapper = null;
        public QueryResult Query(Guid repoId, QueryModel query)
        {
            return new QueryResult(
                                new QueryContext(
                                    _servicesMapper.From(repoId, "Schema"),
                                    _servicesMapper.FromSemver(repoId, "RegistrationsBaseUrl", query.SemVerLevel)),
                                1200000,
                                new List<QueryPackage>
                                {
                    new QueryPackage(
                        _servicesMapper.FromSemver(repoId,"PackageDisplayMetadataUriTemplate",query.SemVerLevel,
                            "packageid","index.json"),
                        "Package",
                        _servicesMapper.FromSemver(repoId,"PackageDisplayMetadataUriTemplate",query.SemVerLevel,
                            "packageid","index.json"),
                        "packageid","1.0.0",
                        new QueryVersion[]
                        {
                            new QueryVersion(
                                _servicesMapper.FromSemver(repoId,"PackageVersionDisplayMetadataUriTemplate",query.SemVerLevel,
                                    "packageid","0.0.9.json"),
                                "0.0.9",1598784)
                        })
                });
            //http://localhost:9080/https/api-v2v3search-0.nuget.org/query?q=RavenDB.server&semVerLevel=2.0.0
        }
    }
}
