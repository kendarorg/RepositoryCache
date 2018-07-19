using Newtonsoft.Json;

namespace NugetProtocol
{
    public class CatalogContext
    {
        public CatalogContext()
        {

        }
        public CatalogContext(string ovocab, string nuget,string schemaTime)
        {
            Ovocab = ovocab;
            Nuget = nuget;

            Items = new ContextObject { OId = "item", OContainer = "@set" };
            Parent = new ContextObject { OType = "@id" };
            CommitTimestamp = new ContextObject { OType = schemaTime };
            Nuget_LastCreated = new ContextObject { OType = schemaTime };
            Nuget_LastEdited = new ContextObject { OType = schemaTime };
            Nuget_LastDeleted = new ContextObject { OType = schemaTime };
        }

        [JsonProperty("@vocab")]
        public string Ovocab { get; set; }
        [JsonProperty("nuget")]
        public string Nuget { get; set; }
        [JsonProperty("items")]
        public ContextObject Items { get; set; }
        [JsonProperty("parent")]
        public ContextObject Parent { get; set; }
        [JsonProperty("commitTimestamp")]
        public ContextObject CommitTimestamp { get; set; }
        [JsonProperty("nuget:lastCreated")]
        public ContextObject Nuget_LastCreated { get; set; }
        [JsonProperty("nuget:lastEdited")]
        public ContextObject Nuget_LastEdited { get; set; }
        [JsonProperty("nuget:lastDeleted")]
        public ContextObject Nuget_LastDeleted { get; set; }
    }
}
