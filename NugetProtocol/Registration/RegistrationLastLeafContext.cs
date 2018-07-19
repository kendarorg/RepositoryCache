namespace NugetProtocol
{
    public class RegistrationLastLeafContext
    {
        public RegistrationLastLeafContext()
        {

        }
        public RegistrationLastLeafContext(string ovocab, string xsd)
        {
            //H:\ProgramFiles\ReposCache\https\api.nuget.org\v3\registration3\ravendb.server\index.json.response
            OVocab = ovocab;
            Xsd = xsd;

            Published = new ContextObject { OType = "xsd:dateTime" };
            CatalogEntry = new ContextObject { OType = "@id" };
            Registration = new ContextObject { OType = "@id" };
            PackageContent = new ContextObject { OType = "@id" };
        }

        public string OVocab { get; set; }
        public string Xsd { get; set; }
        public ContextObject Published { get; set; }
        public ContextObject CatalogEntry { get; set; }
        public ContextObject Registration { get; set; }
        public ContextObject PackageContent { get; set; }
    }
}
