using Newtonsoft.Json;

namespace NugetProtocol
{
    public class CatalogEntryContext
    {
        public CatalogEntryContext()
        {

        }
        public CatalogEntryContext(string ovocab, string catalog, string xsd)
        {
            //H:\ProgramFiles\ReposCache\https\api.nuget.org\v3\registration3\ravendb.server\index.json.response
            OVocab = ovocab;
            Catalog = catalog;
            Xsd = xsd;
            DependencyGroups = new ContextObject { OId = "dependencyGroup", OContainer = "@set" };
            Dependencies = new ContextObject { OId = "dependency", OContainer = "@set" };
            PackageEntries = new ContextObject { OId = "packageEntry", OContainer = "@set" };
            SupportedFrameworks = new ContextObject { OId = "supportedFramework", OContainer = "@set" };
            Tags = new ContextObject { OId = "tag", OContainer = "@set" };
            Published = new ContextObject { OType = "xsd:dateTime" };
            Created = new ContextObject { OType = "xsd:dateTime" };
            LastEdited = new ContextObject { OType = "xsd:dateTime" };
            Catalog_CommitTimestamp = new ContextObject { OType = "xsd:dateTime" };
        }
        
        [JsonProperty("@vocab")]
        public string OVocab { get; set; }
        [JsonProperty("catalog")]
        public string Catalog { get; set; }
        [JsonProperty("xsd")]
        public string Xsd { get; set; }
        [JsonProperty("dependencyGroups")]
        public ContextObject DependencyGroups { get; set; }
        [JsonProperty("dependencies")]
        public ContextObject Dependencies { get; set; }
        [JsonProperty("packageEntries")]
        public ContextObject PackageEntries { get; set; }
        [JsonProperty("supportedFrameworks")]
        public ContextObject SupportedFrameworks { get; set; }
        [JsonProperty("tags")]
        public ContextObject Tags { get; set; }
        [JsonProperty("published")]
        public ContextObject Published { get; set; }
        [JsonProperty("created")]
        public ContextObject Created { get; set; }
        [JsonProperty("lastEdited")]
        public ContextObject LastEdited { get; set; }
        [JsonProperty("catalog:commitTimestamp")]
        public ContextObject Catalog_CommitTimestamp { get; set; }
    }
}
