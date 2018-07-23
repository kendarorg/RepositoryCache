using Newtonsoft.Json;

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

        [JsonProperty("@vocab")]
        public string OVocab { get; set; }
        [JsonProperty("xsd")]
        public string Xsd { get; set; }
        [JsonProperty("published")]
        public ContextObject Published { get; set; }
        [JsonProperty("catalogEntry")]
        public ContextObject CatalogEntry { get; set; }
        [JsonProperty("registration")]
        public ContextObject Registration { get; set; }
        [JsonProperty("packageContent")]
        public ContextObject PackageContent { get; set; }
        
    }
}
