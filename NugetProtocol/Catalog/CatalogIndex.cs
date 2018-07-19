using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class CatalogIndex
    {
        public CatalogIndex()
        {

        }
        public CatalogIndex(string oid, List<string> type,
            Guid commitId, DateTime commitTimeStamp,
            int count, List<CatalogPage> items,
            CatalogContext context,
            DateTime nuget_LastCreated, DateTime nuget_LastEdited, DateTime nuget_LastDeleted)
        {
            OId = oid;
            Type = type;
            CommitId = commitId;
            CommitTimeStamp = commitTimeStamp;
            Count = count;
            Items = items;
            OContext = context;
            Nuget_LastCreated = nuget_LastCreated;
            Nuget_LastEdited = nuget_LastEdited;
            Nuget_LastDeleted = nuget_LastDeleted;
        }

        [JsonProperty("@id")]
        public string OId { get; set; }
        [JsonProperty("type")]
        public List<string> Type { get; set; }
        [JsonProperty("commitId")]
        public Guid CommitId { get; set; }
        [JsonProperty("commitTimestamp")]
        public DateTime CommitTimeStamp { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("items")]
        public List<CatalogPage> Items { get; set; }
        [JsonProperty("@context")]
        public CatalogContext OContext { get; set; }
        [JsonProperty("nuget:lastCreated")]
        public DateTime Nuget_LastCreated { get; set; }
        [JsonProperty("nuget:lastEdited")]
        public DateTime Nuget_LastEdited { get; set; }
        [JsonProperty("nuget:lastDeleted")]
        public DateTime Nuget_LastDeleted { get; set; }
    }
}
