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

        public string OId { get; set; }
        public string OType { get; set; }
        public Guid CommitId { get; set; }
        public DateTime CommitTimestamp { get; set; }
        public PackageDetail CatalogEntry { get; set; }
        public string PackageContent { get; set; }
        public string Registration { get; set; }
        public string HiddenVersion { get; set; }
    }
}
