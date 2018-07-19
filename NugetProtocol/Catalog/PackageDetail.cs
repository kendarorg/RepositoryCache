using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol
{
    public class PackageDetail
    {
        public PackageDetail()
        {
            Authors = new List<string>();
            Owners = new List<string>();
            Tags = new List<string>();
        }

        public PackageDetail(string oid,string otype, string registration, 
            string id, string version,string packageContent,
            string description = null, List<string> authors = null, string iconUrl = null, string licenseUrl = null,
            List<string> owners = null, string projectUrl = null,
            string summary = null, List<string> tags = null, string title = null, int totalDownloads = 0, bool verified = false)
        {
            OId = oid;
            OType = otype;
            Registration = registration;
            Id = id;
            Version = version;
            PackageContent = packageContent;
            Description = description;
            Authors = authors ?? new List<string>();
            IconUrl = iconUrl;
            LicenseUrl = licenseUrl;
            Owners = owners ?? new List<string>();
            ProjectUrl = projectUrl;
            Summary = summary;
            Tags = tags ?? new List<string>();
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
        [JsonProperty("packageContent")]
        public string PackageContent { get; set; }
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
        public int TotalDownloads { get; set; }
        [JsonProperty("verified", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Verified { get; set; }
    }
}
