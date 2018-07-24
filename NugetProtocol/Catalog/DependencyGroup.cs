using Newtonsoft.Json;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class DependencyGroup
    {
        public DependencyGroup()
        {

        }
        public DependencyGroup(string oid,string otype,List<Dependency> dependencies,string targetFramework=null)
        {
            OId = oid;
            OType = otype;
            Dependencies = dependencies;
            TargetFramework = targetFramework;
        }

        [JsonProperty("@id")]
        public string OId { get; set; }
        [JsonProperty("@type")]
        public string OType { get; set; }
        [JsonProperty("dependencies", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Dependency> Dependencies { get; set; }
        [JsonProperty("targetFramework", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string TargetFramework { get; set; }
    }
}
