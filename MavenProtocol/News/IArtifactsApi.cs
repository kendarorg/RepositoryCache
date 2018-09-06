using MavenProtocol;
using MavenProtocol.Apis;

namespace Maven.News
{
    public class ArtifactsApiResult
    {
        public byte[] Content { get; set; }
        public string Md5 { get; set; }
        public string Sha1 { get; set; }
    }
    public interface IArtifactsApi : IMavenApi
    {
        ArtifactsApiResult Retrieve(MavenIndex mi);
        ArtifactsApiResult Generate(MavenIndex mi);
    }
}