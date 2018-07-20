﻿using MultiRepositories.Repositories;
using Nuget.Repositories;
using Nuget.Services;
using NugetProtocol;
using SemVer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Apis
{
    public class NugetSearchQueryService : ISearchQueryService
    {
        private readonly IServicesMapper _servicesMapper;
        private readonly IRepositoryEntitiesRepository _repository;
        private readonly IQueryRepository _queryRepository;

        public NugetSearchQueryService(
            IQueryRepository queryRepository,
            IRepositoryEntitiesRepository repository,
            IServicesMapper servicesMapper)
        {
            _servicesMapper = servicesMapper;
            _repository = repository;
            _queryRepository = queryRepository;
        }

        public QueryResult Query(Guid repoId, QueryModel query)
        {
            var repo = _repository.GetById(repoId);
            var packages = new List<QueryPackage>();
            foreach (var item in _queryRepository.Query(repoId, query))
            {
                var queryVersions = new List<QueryVersion>();
                var shownVersion = item.Version;
                var isSet = false;
                if (query.PreRelease && item.HasPreRelease)
                {
                    shownVersion = item.PreVersion;
                    queryVersions = AddReleaseVersions(repoId, query, item.PreCsvVersion, item.PackageId).ToList();
                    isSet = true;
                    if (item.HasRelease)
                    {
                        if (SemVersion.IsGreater(item.Version, item.PreVersion))
                        {
                            shownVersion = item.Version;
                            queryVersions = AddReleaseVersions(repoId, query, item.CsvVersions, item.PackageId).ToList();
                        }
                    }
                }
                else if (item.HasRelease)
                {
                    isSet = true;
                    shownVersion = item.Version;
                    queryVersions = AddReleaseVersions(repoId, query, item.CsvVersions, item.PackageId).ToList();
                }

                if (!isSet)
                {
                    continue;
                }

                var singleResult = new QueryPackage(
                                        _servicesMapper.FromSemver(repoId, "PackageDisplayMetadataUriTemplate", query.SemVerLevel,
                                            item.PackageId, "index.json"),
                                        "Package",
                                        _servicesMapper.FromSemver(repoId, "PackageDisplayMetadataUriTemplate", query.SemVerLevel,
                                            item.PackageId, "index.json"),
                                        item.PackageId, shownVersion,
                                        queryVersions.ToArray());
                //TODO: Add all the other package data
                packages.Add(singleResult);
            }

            return new QueryResult(
                                new QueryContext(
                                    _servicesMapper.From(repoId, "Schema"),
                                    _servicesMapper.FromSemver(repoId, "RegistrationsBaseUrl", query.SemVerLevel)),
                                99999,
                                packages);
        }

        private IEnumerable<QueryVersion> AddReleaseVersions(Guid repoId, QueryModel query, string versions, string packageId)
        {
            foreach (var singleVersion in versions.Split(','))
            {
                yield return new QueryVersion(
                                        _servicesMapper.FromSemver(repoId, "PackageVersionDisplayMetadataUriTemplate", query.SemVerLevel,
                                            packageId, singleVersion + ".json"),
                                        singleVersion, 0);
            }
        }
    }
}
