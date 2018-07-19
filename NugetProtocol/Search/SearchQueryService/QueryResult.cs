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

        public QueryContext OContext { get; set; }
        /// <summary>
        /// Count for the query
        /// </summary>
        public int TotalHits { get; set; }
        public List<QueryPackage> Data { get; set; }
    }
}
