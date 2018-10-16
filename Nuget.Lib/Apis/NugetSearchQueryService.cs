using Ioc;
using MultiRepositories.Repositories;
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
    public class NugetSearchQueryService : ISearchQueryService, ISingleton
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
            var maxSize = _servicesMapper.MaxQueryPage(repo.Id);
            foreach (var item in _queryRepository.Query(repoId, query))
            {
                if (maxSize == 0)
                {
                    break;
                }
                
                var queryVersions = new List<QueryVersion>();
                var shownVersion = item.Version;
                var isSet = false;
                if (query.PreRelease && item.HasPreRelease)
                {
                    shownVersion = item.PreVersion;
                    queryVersions = PrepareVersions(repoId, query, item.PreCsvVersions, item.PackageId).ToList();
                    isSet = true;
                    if (item.HasRelease)
                    {
                        if (SemVersion.IsGreater(item.Version, item.PreVersion))
                        {
                            shownVersion = item.Version;
                            queryVersions = PrepareVersions(repoId, query, item.CsvVersions, item.PackageId).ToList();
                        }
                    }
                }
                else if (item.HasRelease)
                {
                    isSet = true;
                    shownVersion = item.Version;
                    queryVersions = PrepareVersions(repoId, query, item.CsvVersions, item.PackageId).ToList();
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
                                        queryVersions.ToArray(),
                                        item.Description,
                                        string.IsNullOrWhiteSpace(item.Author)?null:item.Author.Split(new []{',','|'},StringSplitOptions.RemoveEmptyEntries).ToList(),
                                        item.IconUrl,item.LicenseUrl,
                                        string.IsNullOrWhiteSpace(item.Owner) ? null : item.Owner.Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                                        item.ProjectUrl,item.Summary,
                                        string.IsNullOrWhiteSpace(item.Tags) ? null : item.Tags.Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries).ToList(),

                                        item.Title,item.TotalDownloads,item.Verified);
                //TODO: Add all the other package data
                packages.Add(singleResult);
                maxSize--;
            }

            return new QueryResult(
                                new QueryContext(
                                    _servicesMapper.From(repoId, "Schema"),
                                    _servicesMapper.FromSemver(repoId, "RegistrationsBaseUrl", query.SemVerLevel)),
                                99999,
                                packages);
        }

        private IEnumerable<QueryVersion> PrepareVersions(Guid repoId, QueryModel query, string versions, string packageId)
        {
            foreach (var singleVersion in versions.Split('|'))
            {
                if (string.IsNullOrWhiteSpace(singleVersion)) continue;
                yield return new QueryVersion(
                                        _servicesMapper.FromSemver(repoId, "PackageVersionDisplayMetadataUriTemplate", query.SemVerLevel,
                                            packageId, singleVersion + ".json"),
                                        singleVersion, 0);
            }
        }
    }
}
