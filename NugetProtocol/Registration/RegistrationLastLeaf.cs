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

        public string OId { get; set; }
        public List<string> OType { get; set; }
        public bool Listed { get; set; }
        public DateTime Published { get; set; }
        public string CatalogEntry { get; set; }
        public string PackageContent { get; set; }
        public string Registration { get; set; }
        public string HiddenVersion { get; set; }
        public RegistrationLastLeafContext OContext { get; set; }
    }
}
