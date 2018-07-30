using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol.Apis
{
    public class MavenPackageMetadata
    {
        public string Group { get; set; }
        public string ArtifactId { get; set; }
        public string SubType { get; set; }
        public byte[] Content { get; set; }
    }
    public class MavenPackage
    {
        public string Group { get; set; }
        public string ArtifactId { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public byte[] Content { get; set; }
    }
}
