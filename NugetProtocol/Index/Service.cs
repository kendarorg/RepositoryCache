using Newtonsoft.Json;

namespace NugetProtocol
{
    public class Service
    {
        public Service()
        {

        }
        public Service(string oid,string otype,string comment)
        {
            OId = oid;
            OType = otype;
            Comment = comment;
        }

        [JsonProperty("@id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string OId { get; set; }
        [JsonProperty("comment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Comment { get; set; }
        [JsonProperty("@type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string OType { get; set; }
    }
}
