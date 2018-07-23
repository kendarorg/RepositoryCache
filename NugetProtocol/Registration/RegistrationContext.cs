using Newtonsoft.Json;

namespace NugetProtocol
{
    public class RegistrationContext
    {
        public RegistrationContext()
        {

        }
        public RegistrationContext(string ovocab, string catalog, string xsd)
        {
            //H:\ProgramFiles\ReposCache\https\api.nuget.org\v3\registration3\ravendb.server\index.json.response
            OVocab = ovocab;
            Catalog = catalog;
            Xsd = xsd;
            Items = new ContextObject { OId = "catalog:item", OContainer = "@set" };
            CommitTimestamp = new ContextObject { OId = "catalog:commitTimeStamp", OType = "commitTimeStamp" };
            CommitId = new ContextObject { OId = "catalog:commitId" };
            Count = new ContextObject { OId = "catalog:count" };
            Parent = new ContextObject { OId = "catalog:parent", OType = "@id" };
            Tags = new ContextObject { OId = "tag", OContainer = "@set" };
            PackageTargetFrameworks = new ContextObject { OId = "packageTargetFramework", OContainer = "@set" };
            DependencyGroups = new ContextObject { OId = "dependencyGroup", OContainer = "@set" };
            Dependencies = new ContextObject { OId = "dependency", OContainer = "@set" };
            PackageContent = new ContextObject { OType = "@id" };
            Published = new ContextObject { OType = "xsd:dateTime" };
            Registration = new ContextObject { OType = "@id" };
        }
        [JsonProperty("@vocab")]
        public string OVocab { get; set; }
        [JsonProperty("catalog")]
        public string Catalog { get; set; }
        [JsonProperty("xsd")]
        public string Xsd { get; set; }
        [JsonProperty("items")]
        public ContextObject Items { get; set; }
        [JsonProperty("commitTiemstamp")]
        public ContextObject CommitTimestamp { get; set; }
        [JsonProperty("commitId")]
        public ContextObject CommitId { get; set; }
        [JsonProperty("count")]
        public ContextObject Count { get; set; }
        [JsonProperty("parent")]
        public ContextObject Parent { get; set; }
        [JsonProperty("tags")]
        public ContextObject Tags { get; set; }
        [JsonProperty("packageTargetFrameworks")]
        public ContextObject PackageTargetFrameworks { get; set; }
        [JsonProperty("dependencyGroups")]
        public ContextObject DependencyGroups { get; set; }
        [JsonProperty("dependencies")]
        public ContextObject Dependencies { get; set; }
        [JsonProperty("packageContent")]
        public ContextObject PackageContent { get; set; }
        [JsonProperty("published")]
        public ContextObject Published { get; set; }
        [JsonProperty("registration")]
        public ContextObject Registration { get; set; }
    }
}
