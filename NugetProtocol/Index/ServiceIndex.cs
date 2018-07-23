using Newtonsoft.Json;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class ServiceIndex
    {
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("services")]
        public List<Service> Services { get; set; }

        public ServiceIndex(string version, List<Service> services)
        {
            Version = version;
            Services = services;
        }
        public ServiceIndex()
        {
            Services = new List<Service>();
        }
    }
}
