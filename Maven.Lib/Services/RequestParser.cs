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

            string subType = null;
            string version = null;
            string filename = null;
            string package = null;
            string classifier = null;
            string path = string.Empty;
            string type = null;

            if (arg.PathParams.ContainsKey("type")) type = arg.PathParams["type"];

            if (arg.PathParams.ContainsKey("*path")) path = arg.PathParams["*path"];
            if (arg.PathParams.ContainsKey("package")) package = arg.PathParams["package"];
            if (arg.PathParams.ContainsKey("subType")) subType = arg.PathParams["subType"];
            if (arg.PathParams.ContainsKey("filename")) filename = arg.PathParams["filename"];
            if (arg.PathParams.ContainsKey("major")) version = arg.PathParams["major"];
            if (arg.PathParams.ContainsKey("minor")) version += "." + arg.PathParams["minor"];
            if (arg.PathParams.ContainsKey("patch")) version += "." + arg.PathParams["patch"];
            if (arg.PathParams.ContainsKey("extra")) version += "." + arg.PathParams["extra"];
            if (arg.PathParams.ContainsKey("pre")) version += "-" + arg.PathParams["pre"];

            if (arg.PathParams.ContainsKey("filename")) filename = arg.PathParams["filename"];


            if (filename != null && package != null)
            {
                classifier = filename.Replace(package + "-" + version, "");
                if (!string.IsNullOrWhiteSpace(classifier))
                {
                    classifier = classifier.Trim('-');
                }
            }
            return new MavenIndex
            {
                ArtifactId = package,
                Checksum = subType,
                Group = path.Split('/'),
                Version = version,
                Classifier = classifier,
                Filename = filename,
                Type = type
            };
        }
    }
}
