﻿using Nuget.Repositories;
using NugetProtocol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nuget.Apis
{
    public class NugetCatalogService : ICatalogService
    {
        public NugetCatalogService(
            IRegistrationRepository registrationRepository,
            IServicesMapper servicesMapper,
            IPackagesRepository packagesRepository,
            INugetDependenciesRepository nugetDependencies,
            INugetAssembliesRepository nugetAssemblies)
        {
            _nugetAssemblies = nugetAssemblies;
            _nugetDependencies = nugetDependencies;
            _packagesRepository = packagesRepository;
            _registrationRepository = registrationRepository;
            _servicesMapper = servicesMapper;
        }

        private INugetAssembliesRepository _nugetAssemblies;
        private INugetDependenciesRepository _nugetDependencies;
        private IPackagesRepository _packagesRepository;
        private IRegistrationRepository _registrationRepository;
        private IServicesMapper _servicesMapper = null;

        public CatalogIndex GetCatalog(Guid repoId)
        {
            var pageNumber = 0;
            var resultPages = new List<CatalogPage>();
            var pageRegistrationsCount = 0;
            var firstCommitId = Guid.NewGuid();
            var firstTimestamp = DateTime.MinValue;

            var lastCommitId = Guid.NewGuid();
            var lastTimestamp = DateTime.MinValue;
            var maxPerPage = _servicesMapper.MaxCatalogPages(repoId);
            foreach (var registration in _registrationRepository.GetAllOrderByDate(repoId))
            {
                lastCommitId = registration.CommitId;
                lastTimestamp = registration.CommitTimestamp;

                if (pageRegistrationsCount == 0)
                {
                    firstTimestamp = registration.CommitTimestamp;
                    firstCommitId = registration.CommitId;
                }

                pageRegistrationsCount++;
                if (pageRegistrationsCount >= maxPerPage)
                {
                    resultPages.Add(new CatalogPage(
                        _servicesMapper.From(repoId, "Catalog/3.0.0", "index" + pageNumber + ".json"),
                        "CatalogPage",
                        firstCommitId, firstTimestamp,
                        pageRegistrationsCount));
                    pageRegistrationsCount = 0;
                }
            }

            if (pageRegistrationsCount < maxPerPage)
            {
                resultPages.Add(new CatalogPage(
                        _servicesMapper.From(repoId, "Catalog/3.0.0", "index" + pageNumber + ".json"),
                        "CatalogPage",
                        firstCommitId, firstTimestamp,
                        pageRegistrationsCount));
            }

            return new CatalogIndex(
                _servicesMapper.From(repoId, "Catalog/3.0.0", "index.json"),
                new List<string> { "CatalogRoot", "AppendOnlyCatalog", "Permalink" },
                lastCommitId, lastTimestamp,
                resultPages.Count, resultPages,
                new CatalogContext(
                    _servicesMapper.From(repoId, "*Catalog"),
                    _servicesMapper.From(repoId, "*Schema"),
                    _servicesMapper.From(repoId, "*SchemaTime")
                ),
                lastTimestamp, lastTimestamp, lastTimestamp);
        }


        public CatalogPage GetCatalogPage(Guid repoId, int page)
        {

            var result = new List<CatalogPageItem>();

            var lastCommitId = Guid.NewGuid();
            var lastTimestamp = DateTime.MinValue;
            var maxPerPage = _servicesMapper.MaxCatalogPages(repoId);
            foreach (var registration in _registrationRepository.GetPage(repoId, page * maxPerPage, maxPerPage))
            {
                lastCommitId = registration.CommitId;
                lastTimestamp = registration.CommitTimestamp;
                result.Add(new CatalogPageItem(
                                _servicesMapper.From(repoId, "Catalog/3.0.0", "data", registration.CommitTimestamp.ToString("yyyy.MM.dd.HH.mm.ss"),
                                   registration.PackageId + "." + registration.Version + ".json"),
                                "nuget:PackageDetails",
                                lastCommitId, lastTimestamp,
                               registration.PackageId, registration.Version));
            }
            var catalogDate = DateTime.UtcNow;
            return new CatalogPage(
                        _servicesMapper.From(repoId, "Catalog/3.0.0", "index" + page + ".json"),
                        "CatalogPage",
                        lastCommitId, lastTimestamp,
                        result.Count,
                        _servicesMapper.From(repoId, "Catalog/3.0.0", "index.json"),
                        new CatalogContext(
                            _servicesMapper.From(repoId, "*Catalog"),
                            _servicesMapper.From(repoId, "*Schema"),
                            _servicesMapper.From(repoId, "*SchemaTime")
                        ),
                        result
                        );
        }


        public IEnumerable<PackageDetail> GetPackageDetailsForRegistration(Guid repoId, string lowerId, string semVerLevel, params string[] lowerVersions)
        {

            foreach (var item in _packagesRepository.GetByIdVersions(repoId, lowerId, lowerVersions))
            {
                yield return new PackageDetail(
                        _servicesMapper.From(repoId, "Catalog/3.0.0",
                            "data", item.CommitTimestamp.ToString("yyyy.MM.dd.HH.mm.ss"), lowerId + "." + item.Version + ".json"),
                        "PackageDetails",
                        _servicesMapper.FromSemver(repoId, "PackageDisplayMetadataUriTemplate", semVerLevel,
                            lowerId, "index.json"),
                        lowerId, item.Version,
                        _servicesMapper.From(repoId, "PackageBaseAddress/3.0.0",
                            lowerId, item.Version, lowerId + "." + item.Version + ".nupkg")
                        );
            }
        }

        public CatalogEntry GetPackageCatalog(Guid repoId, string timestamp, string idLowerVersionLower)
        {
            var entry = _packagesRepository.GetByIdVersion(repoId, idLowerVersionLower);



            string versionLower = entry.Version;
            string idLower = entry.PackageId;

            List<DependencyGroup> dependencyGroups = FindDependencies(repoId, timestamp, versionLower, idLower);
            List<FrameworkAssemblyGroup> assemblyGroups = FindAssemblyGroups(repoId, timestamp, versionLower, idLower);

            timestamp = entry.CommitTimestamp.ToString("yyyy.MM.dd.HH.mm.ss");
            return new CatalogEntry(
                _servicesMapper.From(repoId, "Catalog/3.0.0", "data", timestamp,
                                    idLower + "." + versionLower + ".json"),
                new List<string> { "PackageDetails", "catalog:Permalink" },
                entry.CommitId, entry.CommitTimestamp,
                idLower, entry.CommitTimestamp, versionLower,
                entry.HashKey, entry.HashAlgorithm, entry.Size,
                new CatalogEntryContext(
                    _servicesMapper.From(repoId, "*Schema"), _servicesMapper.From(repoId, "*Catalog"),
                    _servicesMapper.From(repoId, "*W3SchemaComment")),
                dependencyGroups,
                assemblyGroups);
        }

        private List<FrameworkAssemblyGroup> FindAssemblyGroups(Guid repoId, string timestamp, string versionLower, string idLower)
        {
            var assembliesGroup = new Dictionary<string, List<NugetAssemblyGroup>>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var asm in _nugetAssemblies.GetGroups(repoId, idLower, versionLower))
            {
                var targetFramework = asm.TargetFramework ?? string.Empty;
                if (!assembliesGroup.ContainsKey(targetFramework))
                {
                    assembliesGroup[targetFramework] = new List<NugetAssemblyGroup>();
                }
                assembliesGroup[targetFramework].Add(asm);
            }
            var result = new List<FrameworkAssemblyGroup>();

            foreach (var item in assembliesGroup)
            {
                var fwag = new FrameworkAssemblyGroup(
                         _servicesMapper.From(repoId, "Catalog/3.0.0", "data", timestamp,
                                    idLower + "." + versionLower + ".json" + "#frameworkassemblygroup/" +
                                    item.Key),
                         item.Key,
                         item.Value.Select(a => a.AssemblyName).ToList()
                         );
                result.Add(fwag);
            }

            return result;
        }

        private List<DependencyGroup> FindDependencies(Guid repoId, string timestamp, string versionLower, string idLower)
        {
            var dependencies = _nugetDependencies.GetDependencies(repoId, idLower, versionLower);
            List<DependencyGroup> dependencyGroups = null;
            if (dependencies.Any())
            {
                var internalDeps = new List<Dependency>();
                foreach (var dependency in dependencies)
                {
                    string range = dependency.Version;
                    if (!(range.Contains("[") || range.Contains("]") ||
                        range.Contains("(") || range.Contains(")")))
                    {
                        range = "[" + range + ")";
                    }
                    internalDeps.Add(new Dependency(
                                 _servicesMapper.From(repoId, "Catalog/3.0.0", "data", timestamp,
                                    idLower + "." + versionLower + ".json" + "#dependencygroup/" +
                                    "packageId"),
                                 "PackageDependency",
                                 dependency.PackageId,
                                 range));
                }

                dependencyGroups = new List<DependencyGroup>
                {
                    new DependencyGroup(
                         _servicesMapper.From(repoId, "Catalog/3.0.0", "data", timestamp,
                                    idLower + "." + versionLower + ".json" + "#dependencygroup"),
                         "PackageDependencyGroup", internalDeps)
                };

            }

            return dependencyGroups;
        }
    }
}
