using Newtonsoft.Json;
using System;

namespace NugetProtocol
{
    public class RegistrationLeaf
    {
        public RegistrationLeaf()
        {

        }
        public RegistrationLeaf(string oid, string otype,
            Guid commitId, DateTime commitTimestamp,
            PackageDetail catalogEntry,
            string packageContent, string registration, string hiddenVersion)
        {
            OId = oid;
            OType = otype;
            CommitId = commitId;
            CommitTimestamp = commitTimestamp;
            CatalogEntry = catalogEntry;
            PackageContent = packageContent;
            Registration = registration;
            HiddenVersion = hiddenVersion;
        }

        [JsonProperty("@id")]
        public string OId { get; set; }
        [JsonProperty("@type")]
        public string OType { get; set; }
        [JsonProperty("commitId")]
        public Guid CommitId { get; set; }
        [JsonProperty("commitTimestamp")]
        public DateTime CommitTimestamp { get; set; }
        [JsonProperty("catalogEntry")]
        public PackageDetail CatalogEntry { get; set; }
        [JsonProperty("packageContent")]
        public string PackageContent { get; set; }
        [JsonProperty("registration")]
        public string Registration { get; set; }

        public string HiddenVersion { get; set; }
    }
}
