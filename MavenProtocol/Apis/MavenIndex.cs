using System;

namespace MavenProtocol.Apis
{
    public class MavenIndex
    {
        public string Group { get; set; }
        public string ArtifactId { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }    //maven-metadata.xml
        public string SubType { get; set; }

        public string ToLocalPath()
        {
            return Group.Replace('/', '\\') + "\\" + ArtifactId + "\\" + ArtifactId + "-" + Version + "." + Type;
        }
    }
}
