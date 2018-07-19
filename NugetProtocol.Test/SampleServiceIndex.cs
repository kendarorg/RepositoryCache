using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class SampleServiceIndex : IIndexService
    {
        private IServicesMapper _servicesMapper = null;
        public ServiceIndex Get(Guid repoId)
        {
            return new ServiceIndex("3.0.0", new List<Service>
            {
                new Service(_servicesMapper.From(repoId,"PackagePublish/2.0.0"),"PackagePublish/2.0.0","Publish"),

                new Service(_servicesMapper.From(repoId,"SearchQueryService"),"SearchQueryService","Search"),
                new Service(_servicesMapper.From(repoId,"SearchQueryService/3.0.0-beta"),"SearchQueryService/3.0.0-beta","Search"),
                new Service(_servicesMapper.From(repoId,"SearchQueryService/3.0.0-rc"),"SearchQueryService/3.0.0-rc","Search"),

                new Service(_servicesMapper.From(repoId,"RegistrationsBaseUrl"),"RegistrationsBaseUrl","Registration, semver 1.0.0"),
                new Service(_servicesMapper.From(repoId,"RegistrationsBaseUrl/3.0.0-beta"),"RegistrationsBaseUrl/3.0.0-beta","Registration, semver 1.0.0"),
                new Service(_servicesMapper.From(repoId,"RegistrationsBaseUrl/3.0.0-rc"),"RegistrationsBaseUrl/3.0.0-rc","Registration, semver 1.0.0"),
                new Service(_servicesMapper.From(repoId,"PackageDisplayMetadataUriTemplate/3.0.0-rc","{id-Lower}","index.json"),
                    "PackageDisplayMetadataUriTemplate/3.0.0-rc","Packages metadata"),
                new Service(_servicesMapper.From(repoId,"PackageVersionDisplayMetadataUriTemplate/3.0.0-rc","{id-Lower}","{version-Lower}.json"),
                    "PackageVersionDisplayMetadataUriTemplate/3.0.0-rc","Packages metadata"),



                new Service(_servicesMapper.From(repoId,"RegistrationsBaseUrl/3.4.0"),"RegistrationsBaseUrl/3.4.0","Registration, semver 1.0.0, gz"),

                new Service(_servicesMapper.From(repoId,"RegistrationsBaseUrl/3.6.0"),"RegistrationsBaseUrl/3.6.0","Registration, semver 2.0.0, gz"),

                new Service(_servicesMapper.From(repoId,"SearchQueryService/3.0.0-rc"),"SearchQueryService/3.0.0-rc","Search"),

                new Service(_servicesMapper.From(repoId,"LegacyGallery/2.0.0"),"LegacyGallery/2.0.0","Gallery"),
                new Service(_servicesMapper.From(repoId,"LegacyGallery"),"LegacyGallery","Gallery"),

                new Service(_servicesMapper.From(repoId,"SearchGalleryQueryService/3.0.0-rc"),"SearchGalleryQueryService/3.0.0-rc","Gallery"),
            });
        }
    }
}
