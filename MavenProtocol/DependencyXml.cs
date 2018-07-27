using Newtonsoft.Json;

namespace MavenProtocol
{
    public class DependencyXml
    {
        [JsonProperty("artifactId")]
        public string ArtifactId { get; set; }
        [JsonProperty("groupId")]
        public string GroupId { get; set; }
        [JsonProperty("version", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Version { get; set; }
    }
}