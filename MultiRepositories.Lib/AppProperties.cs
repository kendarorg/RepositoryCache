using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRepositories
{
    public class AppProperties
    {
        public AppProperties(string host, string dbConnectionString)
        {
            Host = host ?? "http://localhost:9080";
            DbConnectionString = dbConnectionString ?? @"db";
        }
        public string Host { get; private set; }
        public string DbConnectionString { get; private set; }
    }
}
