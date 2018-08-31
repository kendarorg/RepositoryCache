
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol
{
    public class MavenSettings
    {
        [JsonProperty("catalogPageSize")]
        public int CatalogPageSize { get; set; }
        [JsonProperty("registrationPageSize")]
        public int RegistrationPageSize { get; set; }
        [JsonProperty("queryPageSize")]
        public int QueryPageSize { get; set; }
        [JsonProperty("remoteAddress")]
        public string RemoteAddress { get; set; }
        [JsonProperty("remoteSearchAddress")]
        public string RemoteSearchAddress { get; set; }
    }
}
