using Newtonsoft.Json;
using System.Collections.Generic;

namespace MavenProtocol.Apis
{
    public class ResponseHeader
    {
        public ResponseHeader(int status, int qtime, Dictionary<string, string> pp)
        {
            Status = status;
            Qtime = qtime;
            Params = pp ?? new Dictionary<string, string>();
        }
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("QTime")]
        public int Qtime { get; set; }
        [JsonProperty("params")]
        public Dictionary<string, string> Params { get; set; }
    }
}