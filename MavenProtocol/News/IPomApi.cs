using Ioc;
using MavenProtocol.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol.News
{
    public class PomApiResult
    {
        public string Md5 { get; set; }
        public string Sha1 { get; set; }
        public PomXml Xml { get; set; }
    }
    public interface IPomApi : IMavenApi
    {
        PomApiResult Retrieve(MavenIndex mi);
        PomApiResult Generate(MavenIndex mi);
    }
}
