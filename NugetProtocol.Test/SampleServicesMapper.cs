using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class SampleServicesMapper : IServicesMapper
    {
        public string BuildDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public string From(Guid repoId, string resourceId, params string[] pars)
        {
            var result = string.Empty;
            switch (resourceId)
            {
                case ("*SchemaTime"):
                    result = "http://www.w3.org/2001/XMLSchema#dateTime";
                    break;
                case ("*CatalogPermalink"):
                    result = "http://schema.nuget.org/catalog#Permalink";
                    break;
                case ("*Catalog"):
                    result = "http://schema.nuget.org/catalog#";
                    break;
                case ("*Schema"):
                    result = "http://schema.nuget.org/schema#";
                    break;
                case ("*W3SchemaComment"):
                    result = "http://www.w3.org/2000/01/rdf-schema#comment";
                    break;
                case ("PackagePublish/2.0.0"):
                    result = "https://www.nuget.org/api/v2/package";
                    break;
                case ("RegistrationsBaseUrl"):
                case ("RegistrationsBaseUrl/3.0.0-beta"):
                case ("RegistrationsBaseUrl/3.0.0-rc"):
                    result = "https://api.nuget.org/v3/registration3"; //no V2
                    break;
                case ("PackageDisplayMetadataUriTemplate/3.0.0-rc"):
                    result = From(repoId,"RegistrationsBaseUrl"); //{id-lower}/index.json no V2
                    break;
                case ("PackageVersionDisplayMetadataUriTemplate/3.0.0-rc"):
                    result = From(repoId,"RegistrationsBaseUrl"); //{id-lower}/{version-lower}.json no V2
                    break;
                case ("RegistrationsBaseUrl/3.4.0"):
                    //result = "https://api.nuget.org/v3/registration3-gz"; //no v2, gz
                    //EDR Assumption
                    result = From(repoId,"RegistrationsBaseUrl"); //no V2
                    break;
                case ("RegistrationsBaseUrl/3.6.0"):
                    result = "https://api.nuget.org/v3/registration3-gz-semver2"; //v2, gz
                    break;
                case ("Catalog/3.0.0"):
                    result = "https://api.nuget.org/v3/catalog0"; //index.json
                    break;
                case ("SearchQueryService"):
                case ("SearchQueryService/3.0.0-beta"):
                case ("SearchQueryService/3.0.0-rc"):
                    result = "https://api-v2v3search-0.nuget.org/query";
                    break;
                case ("PackageBaseAddress/3.0.0"):
                    result = "https://api.nuget.org/v3-flatcontainer";
                    break;
                case ("ReportAbuseUriTemplate"):
                case ("ReportAbuseUriTemplate/3.0.0-beta"):
                case ("ReportAbuseUriTemplate/3.0.0-rc"):
                    result = "https://www.nuget.org/packages"; //{id}/{version}/ReportAbuse
                    break;
                case ("SearchAutocompleteService"):
                case ("SearchAutocompleteService/3.0.0-beta"):
                case ("SearchAutocompleteService/3.0.0-rc"):
                    result = "https://api-v2v3search-0.nuget.org/autocomplete";
                    break;
                case ("LegacyGallery"):
                case ("LegacyGallery/2.0.0"):
                    result = "https://www.nuget.org/api/v2";
                    break;
                case ("SearchGalleryQueryService/3.0.0-rc"):
                    result = "https://api-v2v3search-0.nuget.org";
                    break;
            }
            return result + "/" + string.Join("/", pars);
        }

        public string FromNuget(Guid repoId, string src)
        {
            throw new NotImplementedException();
        }

        public string FromSemver(Guid repoId, string resourceId, string semVerLevel, params string[] par)
        {
            switch (resourceId)
            {
                case ("RegistrationsBaseUrl"):
                    if (string.IsNullOrWhiteSpace(semVerLevel) || semVerLevel == "1.0.0")
                    {
                        return From(repoId,"RegistrationsBaseUrl", par);
                    }
                    else if (semVerLevel == "2.0.0")
                    {
                        return From(repoId,"RegistrationsBaseUrl/3.6.0", par);
                    }
                    throw new NotImplementedException("SemVerLevelNotSupported: " + semVerLevel);
                case ("PackageDisplayMetadataUriTemplate"):
                    if (string.IsNullOrWhiteSpace(semVerLevel) || semVerLevel == "1.0.0")
                    {
                        return From(repoId,"PackageDisplayMetadataUriTemplate/3.0.0-rc", par);
                    }
                    else if (semVerLevel == "2.0.0")
                    {
                        return From(repoId,"RegistrationsBaseUrl/3.6.0", par);
                    }
                    throw new NotImplementedException("SemVerLevelNotSupported: " + semVerLevel);
                case ("PackageVersionDisplayMetadataUriTemplate"):
                    if (string.IsNullOrWhiteSpace(semVerLevel) || semVerLevel == "1.0.0")
                    {
                        return From(repoId,"PackageVersionDisplayMetadataUriTemplate/3.0.0-rc", par);
                    }
                    else if (semVerLevel == "2.0.0")
                    {
                        return From(repoId,"RegistrationsBaseUrl/3.6.0", par);
                    }
                    throw new NotImplementedException("SemVerLevelNotSupported: " + semVerLevel);
                default:
                    return From(repoId,resourceId, par);
            }
        }

        public Dictionary<string, EntryPointDescriptor> GetVisibles(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public string ToNuget(Guid repoId, string src)
        {
            throw new NotImplementedException();
        }
    }
}
