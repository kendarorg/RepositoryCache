using Ioc;
using MavenProtocol.Apis;

namespace MavenProtocol
{
    public interface IMavenApi : ISingleton
    {
        bool CanHandle(MavenIndex mi);
    }
}
