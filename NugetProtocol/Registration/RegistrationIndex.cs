using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class RegistrationIndex
    {
        public RegistrationIndex()
        {
        }
        public RegistrationIndex(
            string oid, List<string> otype,
            Guid commitId, DateTime commitTimestamp,
            int count, List<RegistrationPage> items,
            RegistrationContext ocontext)
        {
            OId = oid;
            OType = otype;
            CommitId = commitId;
            CommitTimestamp = commitTimestamp;
            Count = count;
            Items = items;
            OContext = ocontext;
        }

        [JsonProperty("@id")]
        public string OId { get; set; }
        [JsonProperty("@type")]
        public List<string> OType { get; set; }
        [JsonProperty("commitId")]
        public Guid CommitId { get; set; }
        [JsonProperty("commitTimestamp")]
        public DateTime CommitTimestamp { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("items")]
        public List<RegistrationPage> Items { get; set; }
        [JsonProperty("@context")]
        public RegistrationContext OContext { get; set; }
    }
}
