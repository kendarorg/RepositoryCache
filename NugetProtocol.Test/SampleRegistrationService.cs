using System;
using System.Collections.Generic;
using System.Linq;

namespace NugetProtocol
{
    public class SampleRegistrationService : IRegistrationService
    {

        private IServicesMapper _servicesMapper = null;
        private ICatalogService _catalogService = null;

        /// <summary>
        /// Get The list of registration pages
        /// GET: /{lowerId}/index.json
        /// </summary>
        /// <param name="lowerId"></param>
        /// <param name="withSemVer2"></param>
        /// <returns></returns>
        public RegistrationIndex IndexPage(Guid repoId, string lowerId, string semVerLevel)
        {
            var result = new RegistrationIndex(
                _servicesMapper.FromSemver(repoId, "PackageDisplayMetadataUriTemplate", semVerLevel, lowerId, "index.json"),
                new List<string> { "catalog:CatalogRoot", "PackageRegistration", "catalog:Permalink" },
                Guid.NewGuid(), DateTime.UtcNow,
                64, new List<RegistrationPage>(),
                new RegistrationContext(
                    _servicesMapper.From(repoId, "*Schema"), _servicesMapper.From(repoId, "*Catalog"),
                    _servicesMapper.From(repoId, "*W3SchemaComment"))
                )
            {
                Items = new List<RegistrationPage>
                {
                    new RegistrationPage(
                        _servicesMapper.FromSemver(repoId,"PackageDisplayMetadataUriTemplate",semVerLevel, lowerId,"page","1.0.0","1.5.0"+".json"),
                        "catalog:CatalogPage",
                        Guid.NewGuid(),DateTime.UtcNow,
                        64,"1.0.0","1.5.0",
                         _servicesMapper.FromSemver(repoId,"PackageDisplayMetadataUriTemplate",semVerLevel, lowerId, "index.json"),
                         null,
                         null
                        )
                }
            };
            if (result.Items != null && result.Items.Count == 0)
            {
                result.Items[0] = SinglePage(repoId, lowerId, "1.0.0", "1.5.0", semVerLevel);
            }
            return result;
        }

        public RegistrationPage SinglePage(Guid repoId, string lowerId, string versionFrom, string versionTo, string semVerLevel)
        {
            var result = new RegistrationPage(
                        _servicesMapper.FromSemver(repoId, "PackageDisplayMetadataUriTemplate", semVerLevel, lowerId, "page", versionFrom, versionTo + ".json"),
                        "catalog:CatalogPage",
                        Guid.NewGuid(), DateTime.UtcNow,
                        22, versionFrom, versionTo,
                        _servicesMapper.FromSemver(repoId, "PackageDisplayMetadataUriTemplate", semVerLevel, lowerId, "index.json"),
                        new List<RegistrationLeaf>
                        {
                            new RegistrationLeaf(
                                _servicesMapper.FromSemver(repoId, "PackageDisplayMetadataUriTemplate",semVerLevel, lowerId, "1.0.0"+".json"),
                                "Package",
                                Guid.NewGuid(),DateTime.UtcNow,
                                null,
                                _servicesMapper.From(repoId,"PackageBaseAddress/3.0.0",lowerId,"1.0.0",lowerId+"."+"1.0.0"+".nupkg"),
                                _servicesMapper.FromSemver(repoId, "PackageDisplayMetadataUriTemplate",semVerLevel, lowerId, "index.json"),
                                "1.0.0"
                                )
                        },
                        new RegistrationContext(
                            _servicesMapper.From(repoId, "*Schema"), _servicesMapper.From(repoId, "*Catalog"),
                            _servicesMapper.From(repoId, "*W3SchemaComment")
                        )
                        );
            var versions = result.Items.Select(a => a.HiddenVersion).ToArray();
            var allPackageDetails = _catalogService.GetPackageDetailsForRegistration(repoId, lowerId, semVerLevel, versions).
                ToDictionary(a => a.Version, a => a);
            foreach (var item in result.Items)
            {
                item.CatalogEntry = allPackageDetails[item.HiddenVersion];
            }

            return result;
        }

        public RegistrationLastLeaf Leaf(Guid repoId, string lowerId, string version, string loweridVersion, string semVerLevel)
        {
            var listed = true;
            var catalogDate = DateTime.UtcNow;


            var result = new RegistrationLastLeaf(
                                _servicesMapper.FromSemver(repoId, "PackageDisplayMetadataUriTemplate", semVerLevel, lowerId, version + ".json"),
                                new List<string> { "Package", _servicesMapper.From(repoId, "*CatalogPermalink") },
                                listed, DateTime.UtcNow,
                                _servicesMapper.From(repoId, "Catalog/3.0.0", "data", catalogDate.ToString("yyyy.MM.dd.HH.mm.ss"),
                                    lowerId + "." + version + ".json"),
                                _servicesMapper.From(repoId, "PackageBaseAddress/3.0.0", lowerId, version, lowerId + "." + version + ".nupkg"),
                                _servicesMapper.FromSemver(repoId, "PackageDisplayMetadataUriTemplate", semVerLevel, lowerId, "index.json"),
                                version,
                                new RegistrationLastLeafContext(
                                    _servicesMapper.From(repoId, "*Schema"),
                                    _servicesMapper.From(repoId, "*W3SchemaComment")
                                ));

            return result;
        }
    }
}
