using Newtonsoft.Json;

namespace NugetProtocol
{
    public class Dependency
    {
        public Dependency()
        {

        }
        public Dependency(string oid,string otype,string id, string range)
        {
            OId = oid;
            OType = otype;
            Id = id;
            Range = range;
        }

        [JsonProperty("@id")]
        public string OId { get; set; }
        [JsonProperty("@type")]
        public string OType { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("range")]
        public string Range { get; set; }
    }
}
