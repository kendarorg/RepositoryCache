using Newtonsoft.Json;
using System.Collections.Generic;

namespace MavenProtocol.Apis
{
    public class ResponseContent
    {
        public ResponseContent(int numFound,int start,List<ResponseDoc> docs)
        {
            NumFound = numFound;
            Start = start;
            Docs = docs;
        }
        [JsonProperty("numFound")]
        public int NumFound { get; set; }
        [JsonProperty("start")]
        public int Start { get; set; }
        [JsonProperty("data")]
        public List<ResponseDoc> Docs { get; set; }
    }
}