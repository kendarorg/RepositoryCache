using Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol.Apis.Browse
{
    public interface IMavenExploreService:ISingleton
    {
        ExploreResult Explore(Guid repoId,MavenIndex explore);
    }
}
