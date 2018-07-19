using Newtonsoft.Json;
using System;

namespace NugetProtocol
{
    public class CatalogPageItem
    {
        public CatalogPageItem()
        {

        }
        public CatalogPageItem(string oid, string otype,
            Guid commitId, DateTime commitTimestamp,
            string nuget_Id, string nuget_Version)
        {
            OId = oid;
            OType = otype;
            CommitId = commitId;
            CommitTimestamp = commitTimestamp;
            Nuget_Id = nuget_Id;
            Nuget_Version = nuget_Version;
        }

        [JsonProperty("@id")]
        public string OId { get; set; }
        [JsonProperty("@type")]
        public string OType { get; set; }
        [JsonProperty("commitId")]
        public Guid CommitId { get; set; }
        [JsonProperty("commitTimestamp")]
        public DateTime CommitTimestamp { get; set; }
        [JsonProperty("nuget:id")]
        public string Nuget_Id { get; set; }
        [JsonProperty("nuget:version")]
        public string Nuget_Version { get; set; }
    }
}
