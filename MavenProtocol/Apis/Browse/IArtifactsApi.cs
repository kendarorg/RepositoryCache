using MavenProtocol;
using MavenProtocol.Apis;

namespace Maven.News
{
    public interface IArtifactsApi : IMavenApi
    {
        ArtifactsApiResult Retrieve(MavenIndex mi);
        ArtifactsApiResult Generate(MavenIndex mi);
    }
}