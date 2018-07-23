using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class RegistrationLastLeaf
    {
        public RegistrationLastLeaf()
        {

        }
        public RegistrationLastLeaf(string oid, List<string> otype,
            bool listed, DateTime published,
            string catalogEntry,
            string packageContent, string registration, string hiddenVersion,
            RegistrationLastLeafContext context)
        {
            OId = oid;
            OType = otype;
            Listed = listed;
            Published = published;
            CatalogEntry = catalogEntry;
            PackageContent = packageContent;
            Registration = registration;
            HiddenVersion = hiddenVersion;
            OContext = context;
        }

        [JsonProperty("@id")]
        public string OId { get; set; }
        [JsonProperty("@type")]
        public List<string> OType { get; set; }
        [JsonProperty("listed")]
        public bool Listed { get; set; }
        [JsonProperty("published", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime Published { get; set; }
        [JsonProperty("catalogEntry")]
        public string CatalogEntry { get; set; }
        [JsonProperty("packageContent")]
        public string PackageContent { get; set; }
        [JsonProperty("registration")]
        public string Registration { get; set; }        
        [JsonProperty("@context")]
        public RegistrationLastLeafContext OContext { get; set; }


        public string HiddenVersion { get; set; }
    }
}
