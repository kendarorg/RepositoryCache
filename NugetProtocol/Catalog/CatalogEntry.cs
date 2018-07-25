using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class CatalogEntry
    {
        
        public CatalogEntry(string oid, List<string> otype,
            Guid catalog_CommitId, DateTime catalog_CommitTimestamp,
            string id,
            DateTime published, string version,
            string packageHash, string packageHashAlgorithm, int packageSize,
            CatalogEntryContext context,
            List<DependencyGroup> dependencyGroups ,
            List<FrameworkAssemblyGroup> frameworkAssemblyGroup,
            string description = null, string authors = null, string iconUrl = null,
            string licenseUrl = null, string projectUrl = null,
            bool requireLicenseAcceptance = false,
            string summary = null, List<string> tags = null, string title = null, int totalDownloads = 0,
            bool verified = true,string releaseNotes = null, string verbatimVersion = null)
        {
            OId = oid;
            OType = otype;
            Catalog_CommitId = catalog_CommitId;
            Catalog_CommitTimestamp = catalog_CommitTimestamp;
            Id = id;
            Published = published;
            Version = version;
            PackageHash = packageHash;
            PackageHashAlgorithm = packageHashAlgorithm;
            PackageSize = packageSize;
            OContext = context;
            DependencyGroups = dependencyGroups;
            FrameworkAssemblyGroup = frameworkAssemblyGroup;
            Description = description;
            Authors = authors;
            IconUrl = iconUrl;
            LicenseUrl = licenseUrl;
            ProjectUrl = projectUrl;
            RequireLicenseAcceptance = requireLicenseAcceptance;
            Summary = summary;
            Tags = tags;
            Title = title;
            TotalDownloads = totalDownloads;
            Verified = verified;
            ReleaseNotes = releaseNotes;
            VerbatimVersion = verbatimVersion;
        }

        [JsonProperty("@id")]
        public string OId { get; set; }
        [JsonProperty("@type")]
        public List<string> OType { get; set; }
        [JsonProperty("catalog:commitId")]
        public Guid Catalog_CommitId { get; set; }
        [JsonProperty("catalog:commitTimestamp")]
        public DateTime Catalog_CommitTimestamp { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("published")]
        public DateTime Published { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("packageHash")]
        public string PackageHash { get; set; }
        [JsonProperty("packageHashAlgorithm")]
        public string PackageHashAlgorithm { get; set; }
        [JsonProperty("packageSize")]
        public int PackageSize { get; set; }
        [JsonProperty("@context")]
        public CatalogEntryContext OContext { get; set; }

        [JsonProperty("dependencyGroups", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<DependencyGroup>))]
        public List<DependencyGroup> DependencyGroups { get; set; }

        [JsonProperty("frameworkAssemblyGroup", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<FrameworkAssemblyGroup>))]
        public List<FrameworkAssemblyGroup> FrameworkAssemblyGroup { get; set; }

        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }
        [JsonProperty("authors", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Authors { get; set; }
        [JsonProperty("iconUrl", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string IconUrl { get; set; }
        [JsonProperty("licenseUrl", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LicenseUrl { get; set; }
        [JsonProperty("projectUrl", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ProjectUrl { get; set; }
        [JsonProperty("requireLicenseAcceptance", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool RequireLicenseAcceptance { get; set; }
        [JsonProperty("summary", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Summary { get; set; }
        [JsonProperty("tags", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Tags { get; set; }
        [JsonProperty("title", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Title { get; set; }
        [JsonProperty("totalDownloads", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int TotalDownloads { get; set; }
        [JsonProperty("verified", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Verified { get; set; }
        [JsonProperty("releaseNotes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ReleaseNotes { get; }
        [JsonProperty("verbatimVersion", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string VerbatimVersion { get; }
    }
}
