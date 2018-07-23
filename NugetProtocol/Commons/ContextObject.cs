using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol
{
    public class ContextObject
    {
        [JsonProperty("@id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string OId { get; set; }
        [JsonProperty("@container", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string OContainer { get; set; }
        [JsonProperty("@type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string OType { get; set; }
    }
}
