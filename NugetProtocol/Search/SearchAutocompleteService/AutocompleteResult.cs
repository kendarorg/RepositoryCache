using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class AutocompleteResult
    {
        public AutocompleteResult()
        {

        }
        public AutocompleteResult(AutocompleteContext ocontext, int totalHits, DateTime lastReopen, string index,
            List<string> data)
        {
            OContext = ocontext;
            TotalHits = totalHits;
            LastReopen = lastReopen;
            Index = index;
            Data = data;
        }

        [JsonProperty("@context")]
        public AutocompleteContext OContext { get; }
        [JsonProperty("totalHits")]
        public int TotalHits { get; }
        [JsonProperty("lastReopen")]
        public DateTime LastReopen { get; }
        [JsonProperty("index")]
        public string Index { get; }
        [JsonProperty("data", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Data { get; }
    }
}
