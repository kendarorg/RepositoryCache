using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maven.News;
using MavenProtocol.Apis;
using MultiRepositories;

namespace Maven.Services
{
    public class RequestParser : IRequestParser
    {
        
        public MavenIndex Parse(SerializableRequest arg)
        {

            string path = null, repoId = null, package = null,
                version = null, specifier = null, extension = null,
                checksum = null, meta = null, filename = null, build = null;
            var isSnapData = false;

            if (arg.PathParams.ContainsKey("snapshot")) isSnapData = true;
            if (arg.PathParams.ContainsKey("path")) path = arg.PathParams["path"];
            if (arg.PathParams.ContainsKey("repoId")) repoId = arg.PathParams["repoId"];
            if (arg.PathParams.ContainsKey("package")) package = arg.PathParams["package"];
            if (arg.PathParams.ContainsKey("version")) version = arg.PathParams["version"];
            if (arg.PathParams.ContainsKey("specifier")) specifier = arg.PathParams["specifier"];
            if (arg.PathParams.ContainsKey("extension")) extension = arg.PathParams["extension"];
            if (arg.PathParams.ContainsKey("checksum")) checksum = arg.PathParams["checksum"];
            if (arg.PathParams.ContainsKey("meta")) meta = arg.PathParams["meta"];
            if (arg.PathParams.ContainsKey("build")) build = arg.PathParams["build"];



            BuildDirtyChecksum(ref extension, ref checksum, "md5");
            BuildDirtyChecksum(ref extension, ref checksum, "sha1");
            BuildDirtyChecksum(ref extension, ref checksum, "asc");

            if (!string.IsNullOrWhiteSpace(meta))
            {
                filename = meta;
            }
            else if (!string.IsNullOrWhiteSpace(extension))
            {
                filename = string.Format("{0}-{1}.{2}{3}",
                    package, version, extension, checksum == null ? "" : "." + checksum);
            }

            var isSnapshot = false;
            if (!string.IsNullOrWhiteSpace(version) && version.EndsWith("-SNAPSHOT"))
            {
                version = version.Replace("-SNAPSHOT", "");
                isSnapshot = true;
            }
            if (isSnapData)
            {
                isSnapshot = true;
            }
            if (string.IsNullOrWhiteSpace(path))
            {
                path = string.Empty;
            }

            var result = new MavenIndex
            {
                ArtifactId = package,
                Checksum = checksum,
                Group = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries),
                Version = version,
                Classifier = specifier,
                IsSnapshot = isSnapshot,
                Filename = filename,
                Extension = extension,
                Meta = meta,
                Content = arg.Content
            };

            if (!string.IsNullOrWhiteSpace(build))
            {
                var splitted = build.Split('-');
                result.Build = splitted.Last();
                build = build.Substring(0, build.Length - result.Build.Length-1);
                result.Timestamp = DateTime.ParseExact(build, "yyyyMMdd.HHmmss", CultureInfo.InvariantCulture);
            }

            return result;
        }

        private static void BuildDirtyChecksum(ref string extension, ref string checksum, string md5)
        {
            if (string.IsNullOrWhiteSpace(extension)) return;
            if (extension.EndsWith("." + md5))
            {
                extension = extension.Replace("." + md5, "");
                checksum = md5;
            }
        }
    }
}
