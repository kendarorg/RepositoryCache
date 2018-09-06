using Ioc;
using MavenProtocol.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol.News
{
    public class ExploreResponse
    {
        public List<string> Children { get; set; }
    }
    public interface IExploreApi : ISingleton
    {
        ExploreResponse Retrieve(MavenIndex mi);
    }
}
