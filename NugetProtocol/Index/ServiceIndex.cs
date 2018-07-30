using Newtonsoft.Json;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class IndexCatalog
    {
        public IndexCatalog()
        {

        }
        public IndexCatalog(string ovocab, string comment)
        {
            Ovocab = ovocab;
            Comment = comment;
        }

        [JsonProperty("@vocab")]
        public string Ovocab { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
    public class ServiceIndex
    {
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("resources")]
        public List<Service> Services { get; set; }
        [JsonProperty("@catalog")]
        public IndexCatalog OCatalog { get; set; }

        public ServiceIndex(string version, List<Service> services,IndexCatalog catalog)
        {
            Version = version;
            Services = services;
            OCatalog = catalog;
        }
        public ServiceIndex()
        {
            Services = new List<Service>();
        }
    }
}
