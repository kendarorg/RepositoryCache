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
            string summary = null, List<string> tags = null, string title = null, int totalDownloads = 0, bool verified = true)
        {
            Id = id;
            Version = version;
            Versions = versions.ToList();
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

        public string Id { get; set; }
        public string Version { get; set; }
        public List<QueryVersion> Versions { get; set; }
        public string Description { get; set; }
        public List<string> Authors { get; set; }
        public string IconUrl { get; set; }
        public string LicenseUrl { get; set; }
        public List<string> Owners { get; set; }
        public string ProjectUrl { get; set; }
        public string Summary { get; set; }
        public List<string> Tags { get; set; }
        public string Title { get; set; }
        public int TotalDownloads { get; set; }
        public bool Verified { get; set; }
    }
}
