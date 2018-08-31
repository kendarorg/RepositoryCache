using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MavenProtocol.Apis;
using MultiRepositories;

namespace Maven.Services
{
    public class RequestParser : IRequestParser
    {
        public MavenIndex Parse(SerializableRequest arg)
        {

            string path=null, repoId = null, package = null, 
                version = null, specifier = null, extension = null, 
                checksum = null, meta = null,filename = null;

            if (arg.PathParams.ContainsKey("path")) path = arg.PathParams["path"];
            if (arg.PathParams.ContainsKey("repoId")) repoId = arg.PathParams["repoId"];
            if (arg.PathParams.ContainsKey("package")) package = arg.PathParams["package"];
            if (arg.PathParams.ContainsKey("version")) version = arg.PathParams["version"];
            if (arg.PathParams.ContainsKey("specifier")) specifier = arg.PathParams["specifier"];
            if (arg.PathParams.ContainsKey("extension")) extension = arg.PathParams["extension"];
            if (arg.PathParams.ContainsKey("checksum")) checksum = arg.PathParams["checksum"];
            if (arg.PathParams.ContainsKey("meta")) meta = arg.PathParams["meta"];

            
            if (!string.IsNullOrWhiteSpace(meta))
            {
                filename = meta;
            }
            else if(!string.IsNullOrWhiteSpace(extension))
            {
                filename = string.Format("{0}-{1}.{2}{3}", 
                    package, version, extension,checksum==null?"":"."+checksum);
            }

            var isSnapshot = false;
            if (version.EndsWith("-SNAPSHOT"))
            {
                version = version.Replace("-SNAPSHOT", "");
                isSnapshot = true;
            }
            
            return new MavenIndex
            {
                ArtifactId = package,
                Checksum = checksum,
                Group = path.Split('/'),
                Version = version,
                Classifier = specifier,
                IsSnapshot = isSnapshot,
                Filename = filename,
                Type = extension
            };
        }
    }
}
