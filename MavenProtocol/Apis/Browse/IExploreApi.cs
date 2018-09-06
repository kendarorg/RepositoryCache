using Ioc;
using MavenProtocol.Apis;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol.News
{
    public interface IExploreApi : ISingleton
    {
        ExploreResponse Retrieve(MavenIndex mi);
    }
}
