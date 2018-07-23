using Newtonsoft.Json;

namespace NugetProtocol
{
    public class QueryContext
    {
        public QueryContext()
        {

        }
        public QueryContext(string ovocab, string obase)
        {
            OVocab = ovocab;
            OBase = obase;
        }
        [JsonProperty("@vocab")]
        public string OVocab { get; set; }
        [JsonProperty("@base")]
        public string OBase { get; set; }
    }
}
