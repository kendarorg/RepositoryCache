using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol
{
    public class QueryModel
    {
        public QueryModel()
        {

        }
        public QueryModel(string query, int skip, int take, bool preRelease, string semVerLevel)
        {
            Query = query;
            Skip = skip;
            Take = take;
            PreRelease = preRelease;
            SemVerLevel = semVerLevel;
        }

        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("skip")]
        public int Skip { get; set; }
        [JsonProperty("take")]
        public int Take { get; set; }
        [JsonProperty("prerelease")]
        public bool PreRelease { get; set; }
        [JsonProperty("semverlevel")]
        public string SemVerLevel { get; set; }
    }
}
