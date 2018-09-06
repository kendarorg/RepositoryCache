using MavenProtocol.Apis;

namespace MavenProtocol
{
    public interface IMetadataApi : IMavenApi
    {
        MetadataApiResult Retrieve(MavenIndex mi);
        MetadataApiResult Generate(MavenIndex mi);
    }
}
