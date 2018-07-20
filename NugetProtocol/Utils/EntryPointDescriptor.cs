using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol
{
    public class EntryPointDescriptor
    {
        public EntryPointDescriptor()
        {
            Compress = false;
            //Visible = true;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("comment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Comment { get; set; }

        /// <summary>
        /// /v3/registration
        /// </summary>
        [JsonProperty("local")]
        public string Local { get; set; }

        /// <summary>
        /// https://api.nuget.org/v3/registration3
        /// </summary>
        [JsonProperty("remote", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Remote { get; set; }

        /// <summary>
        /// https://api.nuget.org/v3/registration3
        /// </summary>
        [JsonProperty("altremote", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string RemoteAlternative { get; set; }

        /// <summary>
        /// 2.0.0, 1.0.0
        /// </summary>
        [JsonProperty("semver", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SemVer { get; set; }

        /// <summary>
        /// true/false
        /// </summary>
        [JsonProperty("compress", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Compress { get; set; }

        /// <summary>
        /// Reference to another type
        /// </summary>
        [JsonProperty("ref", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Ref { get; set; }


        /// <summary>
        /// Reference to another type
        /// </summary>
        [JsonProperty("visible", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Visible { get; set; }
    }
}
