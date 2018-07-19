using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class CatalogPage
    {
        public CatalogPage()
        {

        }
        public CatalogPage(string oid, string otype,
             Guid commitId, DateTime commitTimeStamp, int count,
             string parent = null, CatalogContext context = null,
             List<CatalogPageItem> items = null)
        {
            OId = oid;
            OType = otype;
            CommitId = commitId;
            CommitTimeStamp = commitTimeStamp;
            Count = count;
            Parent = parent;
            OContext = context;
            Items = items;
        }

        [JsonProperty("@id")]
        public string OId { get; set; }
        [JsonProperty("@type")]
        public string OType { get; set; }
        [JsonProperty("commitId")]
        public Guid CommitId { get; set; }
        [JsonProperty("commitTimestamp")]
        public DateTime CommitTimeStamp { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("parent")]
        public string Parent { get; set; }
        [JsonProperty("@context")]
        public CatalogContext OContext { get; set; }
        [JsonProperty("items")]
        public List<CatalogPageItem> Items { get; set; }
    }
}
