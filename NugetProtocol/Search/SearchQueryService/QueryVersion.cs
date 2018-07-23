using Newtonsoft.Json;

namespace NugetProtocol
{
    public class QueryVersion
    {
        public QueryVersion()
        {

        }
        public QueryVersion(string oid, string version, int downloads = 0)
        {
            OId = oid;
            Version = version;
            Downloads = downloads;
        }

        [JsonProperty("@id")]
        public string OId { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("downloads")]
        public int Downloads { get; set; }
    }
}
