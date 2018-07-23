using Newtonsoft.Json;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class QueryResult
    {
        public QueryResult()
        {

        }
        public QueryResult(QueryContext context, int totalHits, List<QueryPackage> data)
        {
            OContext = context;
            TotalHits = totalHits;
            Data = data;
        }

        [JsonProperty("@context")]
        public QueryContext OContext { get; set; }
        /// <summary>
        /// Count for the query
        /// </summary>
        [JsonProperty("totalHits")]
        public int TotalHits { get; set; }
        [JsonProperty("data")]
        public List<QueryPackage> Data { get; set; }
    }
}
