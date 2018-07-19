using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class SampleCatalogService : ICatalogService
    {
        private IServicesMapper _servicesMapper = null;
        public CatalogIndex GetCatalog(Guid repoId)
        {
            var pageNumber = 0;
            return new CatalogIndex(
                _servicesMapper.From(repoId,"Catalog/3.0.0", "index.json"),
                new List<string> { "CatalogRoot", "AppendOnlyCatalog", "Permalink" },
                Guid.NewGuid(), DateTime.UtcNow,
                4089, new List<CatalogPage>
                {
                    new CatalogPage(
                        _servicesMapper.From(repoId,"Catalog/3.0.0","index"+pageNumber+".json"),
                        "CatalogPage",
                        Guid.NewGuid(),DateTime.UtcNow,
                        594)
                },
                new CatalogContext(
                    _servicesMapper.From(repoId,"*Catalog"), 
                    _servicesMapper.From(repoId,"*Schema"),
                    _servicesMapper.From(repoId,"*SchemaTime")
                ),
                DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow);
        }

        public IEnumerable<PackageDetail> GetPackageDetailsForRegistration(Guid repoId,string lowerId, string semVerLevel, params string[] lowerVersions)
        {
            foreach (var version in lowerVersions)
            {
                yield return new PackageDetail(
                        _servicesMapper.From(repoId,"Catalog/3.0.0",
                            "data", "2015.02.01.07.18.10", lowerId + "." + version + ".json"),
                        "PackageDetails",
                        _servicesMapper.FromSemver(repoId, "PackageDisplayMetadataUriTemplate", semVerLevel,
                            lowerId, "index.json"),
                        lowerId, version,
                        _servicesMapper.From(repoId,"PackageBaseAddress/3.0.0",
                            lowerId, version, lowerId + "." + version + ".nupkg")
                        );
            }
        }

        public CatalogPage GetCatalogPage(Guid repoId,int page)
        {
            var catalogDate = DateTime.UtcNow;
            return new CatalogPage(
                        _servicesMapper.From(repoId,"Catalog/3.0.0", "index" + page + ".json"),
                        "CatalogPage",
                        Guid.NewGuid(), DateTime.UtcNow,
                        594,
                        _servicesMapper.From(repoId,"Catalog/3.0.0", "index.json"),
                        new CatalogContext(
                            _servicesMapper.From(repoId,"*Catalog"),
                            _servicesMapper.From(repoId,"*Schema"),
                            _servicesMapper.From(repoId,"*SchemaTime")
                        ),
                        new List<CatalogPageItem>
                        {
                            new CatalogPageItem(
                                _servicesMapper.From(repoId,"Catalog/3.0.0", "data", catalogDate.ToString("yyyy.MM.dd.HH.mm.ss"),
                                    "packageid" + "." + "1.0.0" + ".json"),
                                "nuget:PackageDetails",
                                Guid.NewGuid(),catalogDate,
                                "packageid","1.0.0")
                        }
                        );
        }

        public CatalogEntry GetPackageCatalog(Guid repoId,string timestamp, string idLowerVersionLower)
        {
            string versionLower = BuildVersion(idLowerVersionLower);
            string idLower = BuildId(idLowerVersionLower);

            return new CatalogEntry(
                _servicesMapper.From(repoId,"Catalog/3.0.0", "data", timestamp,
                                    idLower + "." + versionLower + ".json"),
                new List<string> { "PackageDetails", "catalog:Permalink" },
                Guid.NewGuid(), DateTime.UtcNow,
                idLower, DateTime.UtcNow, versionLower,
                "AB12354EAECB==", "SHA512", 123456,
                new CatalogEntryContext(
                    _servicesMapper.From(repoId,"*Schema"), _servicesMapper.From(repoId,"*Catalog"),
                    _servicesMapper.From(repoId,"*W3SchemaComment")),
                new List<DependencyGroup>
                {
                    new DependencyGroup(
                         _servicesMapper.From(repoId,"Catalog/3.0.0", "data", timestamp,
                                    idLower + "." + versionLower + ".json"+"#dependencygroup"),
                         "PackageDependencyGroup",
                         new List<Dependency>{
                             new Dependency(
                                 _servicesMapper.From(repoId,"Catalog/3.0.0", "data", timestamp,
                                    idLower + "." + versionLower + ".json"+"#dependencygroup/"+
                                    "packageId"),
                                 "PackageDependency",
                                 "packageId",
                                 "[1.0.0)"
                                 )
                         })
                }
                , new List<FrameworkAssemblyGroup>
                {
                    new FrameworkAssemblyGroup(
                         _servicesMapper.From(repoId,"Catalog/3.0.0", "data", timestamp,
                                    idLower + "." + versionLower + ".json"+"#frameworkassemblygroup/"+
                                    ".netframework4.0"),
                         ".NETFramework4.0",
                         new List<string>{ "System.Web" }
                         )
                }
                );
        }

        private string BuildId(string idLowerVersionLower)
        {
            throw new NotImplementedException();
        }

        private string BuildVersion(string idLowerVersionLower)
        {
            throw new NotImplementedException();
        }
    }
}
