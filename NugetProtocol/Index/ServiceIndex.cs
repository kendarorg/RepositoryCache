using System.Collections.Generic;

namespace NugetProtocol
{
    public class ServiceIndex
    {
        public string Version { get; set; }
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
