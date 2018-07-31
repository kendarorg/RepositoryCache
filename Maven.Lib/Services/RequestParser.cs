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
            if (!arg.PathParams.ContainsKey("subtype"))
            {
                arg.PathParams["subtype"] = string.Empty;
            }

            var subType = arg.PathParams["subtype"];
            var version = "";
            if (arg.PathParams.ContainsKey("major")) version += arg.PathParams["major"];
            if (arg.PathParams.ContainsKey("minor")) version += "." + arg.PathParams["major"];
            if (arg.PathParams.ContainsKey("patch")) version += "." + arg.PathParams["patch"];
            if (arg.PathParams.ContainsKey("extra")) version += "." + arg.PathParams["extra"];
            if (arg.PathParams.ContainsKey("pre")) version += "-" + arg.PathParams["pre"];



            var classifier = arg.PathParams["filename"].Replace(arg.PathParams["package"], "");
            if (string.IsNullOrWhiteSpace(classifier))
            {
                classifier = null;
            }
            return new MavenIndex
            {
                ArtifactId = arg.PathParams["package"],
                Checksum = arg.PathParams["subtype"],
                Group = arg.PathParams["*path"].Split('/'),
                Version = version,
                Classifier = classifier
            };
        }
    }
}
