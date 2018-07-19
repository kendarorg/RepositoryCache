using NugetProtocol;
using NugetV3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetV3.Apis
{
    public class SearchQueryService : ISearchQueryService
    {
        private readonly IQueryBuilder _queryBuilder;
        private readonly IServicesMapper _servicesMapper;

        public SearchQueryService(IQueryBuilder queryBuilder, IServicesMapper servicesMapper)
        {
            this._queryBuilder = queryBuilder;
            this._servicesMapper = servicesMapper;
        }

        public QueryResult Query(QueryModel query)
        {
            var parsedQuery = _queryBuilder.ParseQuery(query.Query);
            var semVerLevel = query.SemVerLevel;



            return new QueryResult(
                                new QueryContext(
                                    _servicesMapper.From(repoId,"Schema"),
                                    _servicesMapper.FromSemver("RegistrationsBaseUrl", semVerLevel)),
                                1200000,
                                new List<QueryPackage>
                                {
                    new QueryPackage(
                        _servicesMapper.FromSemver("PackageDisplayMetadataUriTemplate",semVerLevel,
                            "packageid","index.json"),
                        "Package",
                        _servicesMapper.FromSemver("PackageDisplayMetadataUriTemplate",semVerLevel,
                            "packageid","index.json"),
                        "packageid","1.0.0",
                        new QueryVersion[]
                        {
                            new QueryVersion(
                                _servicesMapper.From(repoId,"PackageVersionDisplayMetadataUriTemplate",semVerLevel,
                                    "packageid","0.0.9.json"),
                                "0.0.9",1598784)
                        })
                });
        }
    }
}
