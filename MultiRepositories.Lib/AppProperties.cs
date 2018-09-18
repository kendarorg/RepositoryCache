using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        public bool IsOnline(SerializableRequest req)
        {
            if (req.QueryParams.Any(p => p.Key == "offline")) return false;
            try
            {
                var html = string.Empty;
                string url = "https://www.google.com";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
