namespace NugetProtocol
{
    public class RegistrationContext
    {
        public RegistrationContext()
        {

        }
        public RegistrationContext(string ovocab, string catalog, string xsd)
        {
            //H:\ProgramFiles\ReposCache\https\api.nuget.org\v3\registration3\ravendb.server\index.json.response
            OVocab = ovocab;
            Catalog = catalog;
            Xsd = xsd;
            Items = new ContextObject { OId = "catalog:item", OContainer = "@set" };
            CommitTimestamp = new ContextObject { OId = "catalog:commitTimeStamp", OType = "commitTimeStamp" };
            CommitId = new ContextObject { OId = "catalog:commitId" };
            Count = new ContextObject { OId = "catalog:count" };
            Parent = new ContextObject { OId = "catalog:parent", OType = "@id" };
            Tags = new ContextObject { OId = "tag", OContainer = "@set" };
            PackageTargetFrameworks = new ContextObject { OId = "packageTargetFramework", OContainer = "@set" };
            DependencyGroups = new ContextObject { OId = "dependencyGroup", OContainer = "@set" };
            Dependencies = new ContextObject { OId = "dependency", OContainer = "@set" };
            PackageContent = new ContextObject { OType = "@id" };
            Published = new ContextObject { OType = "xsd:dateTime" };
            Registration = new ContextObject { OType = "@id" };
        }
        public string OVocab { get; set; }
        public string Catalog { get; set; }
        public string Xsd { get; set; }
        public ContextObject Items { get; set; }
        public ContextObject CommitTimestamp { get; set; }
        public ContextObject CommitId { get; set; }
        public ContextObject Count { get; set; }
        public ContextObject Parent { get; set; }
        public ContextObject Tags { get; set; }
        public ContextObject PackageTargetFrameworks { get; set; }
        public ContextObject DependencyGroups { get; set; }
        public ContextObject Dependencies { get; set; }
        public ContextObject PackageContent { get; set; }
        public ContextObject Published { get; set; }
        public ContextObject Registration { get; set; }
    }
}
