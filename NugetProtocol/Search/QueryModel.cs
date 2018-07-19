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

        public string Query { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool PreRelease { get; set; }
        public string SemVerLevel { get; set; }
    }
}
