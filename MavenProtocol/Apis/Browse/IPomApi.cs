using Ioc;
using MavenProtocol.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol.News
{
    public interface IPomApi : IMavenApi
    {
        PomApiResult Retrieve(MavenIndex mi);
        PomApiResult Generate(MavenIndex mi);
    }
}
