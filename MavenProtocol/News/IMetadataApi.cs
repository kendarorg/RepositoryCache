using MavenProtocol.Apis;

namespace MavenProtocol
{
    public class MetadataApiResult
    {
        public MavenMetadataXml Xml { get; set; }
        public string Md5 { get; set; }
        public string Sha1 { get; set; }
    }
    public interface IMetadataApi : IMavenApi
    {
        MetadataApiResult Retrieve(MavenIndex mi);
        MetadataApiResult Generate(MavenIndex mi);
    }
}
