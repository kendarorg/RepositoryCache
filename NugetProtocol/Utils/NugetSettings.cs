using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol.Utils
{
    public class NugetSettings
    {
        [JsonProperty("catalogPageSize")]
        public int CatalogPageSize { get; set; }
        [JsonProperty("registrationPageSize")]
        public int RegistrationPageSize { get; set; }
        [JsonProperty("services")]
        public List<EntryPointDescriptor> Services { get; set; }
    }
}
