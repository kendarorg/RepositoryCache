using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRepositories
{
    public class AppProperties
    {
        public AppProperties()
        {
            Host = "http://localhost:9080";
            NupkgDir = @"data\nupkg";
            DbDir = @"data\db";
            MaxRegistryPageSize = 64;
            RunLocal = true;
        }
        public bool RunLocal { get; private set; }
        public string Host { get; private set; }
        public string NupkgDir { get; private set; }
        public string DbDir { get; private set; }
        public int MaxRegistryPageSize { get; private set; }
    }
}
