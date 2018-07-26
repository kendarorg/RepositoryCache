using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace NugetProtocol
{
    public class QueryPackage
    {
        public QueryPackage()
        {
            Versions = new List<QueryVersion>();
            Authors = new List<string>();
            Owners = new List<string>();
            Tags = new List<string>();
        }

        public QueryPackage(string oid, string otype, string registration, string id, string version, QueryVersion[] versions,
            string description = null, List<string> authors = null, string iconUrl = null, string licenseUrl = null,
            List<string> owners = null, string projectUrl = null,
            string summary = null, List<string> tags = null, string title = null, long totalDownloads = 0, bool verified = true)
        {
            OId = oid;
            OType = otype;
            Registration = registration;
            Id = id;
            Version = version;
            Versions = versions.ToList();
            Description = description;
            Authors = authors ?? null;
            IconUrl = iconUrl;
            LicenseUrl = licenseUrl;
            Owners = owners ?? null;
            ProjectUrl = projectUrl;
            Summary = summary;
            Tags = tags ?? null;
            Title = title;
            TotalDownloads = totalDownloads;
            Verified = verified;
        }

        [JsonProperty("@id")]
        public string OId { get; set; }
        [JsonProperty("@type")]
        public string OType { get; set; }
        [JsonProperty("registration")]
        public string Registration { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("versions")]
        public List<QueryVersion> Versions { get; set; }
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }
        [JsonProperty("authors", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Authors { get; set; }
        [JsonProperty("iconUrl", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string IconUrl { get; set; }
        [JsonProperty("licenseUrl", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LicenseUrl { get; set; }
        [JsonProperty("owners", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Owners { get; set; }
        [JsonProperty("projectUrl", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ProjectUrl { get; set; }
        [JsonProperty("summary", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Summary { get; set; }
        [JsonProperty("tags", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Tags { get; set; }
        [JsonProperty("title", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Title { get; set; }
        [JsonProperty("totalDownloads", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long TotalDownloads { get; set; }
        [JsonProperty("verified", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Verified { get; set; }
    }
}
