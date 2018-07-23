using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class RegistrationPage
    {
        public RegistrationPage()
        {

        }
        public RegistrationPage(
            string oid, string otype,
            Guid commitId, DateTime commitTimestamp, int count,
            string lower, string upper,
            string parent,
            List<RegistrationLeaf> items,
            RegistrationContext context)
        {
            OId = oid;
            OType = otype;
            CommitId = commitId;
            CommitTimestamp = commitTimestamp;
            Count = count;
            Lower = lower;
            Upper = upper;
            Parent = parent;
            Items = items;
            OContext = context;
        }

        [JsonProperty("@id")]
        public string OId { get; set; }
        [JsonProperty("@type")]
        public string OType { get; set; }
        [JsonProperty("commitId")]
        public Guid CommitId { get; set; }
        [JsonProperty("commitTimestamp")]
        public DateTime CommitTimestamp { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("lower")]
        public string Lower { get; set; }
        [JsonProperty("upper")]
        public string Upper { get; set; }
        [JsonProperty("parent")]
        public string Parent { get; set; }
        [JsonProperty("items", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<RegistrationLeaf> Items { get; set; }
        [JsonProperty("@context", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public RegistrationContext OContext { get; set; }
    }
}
