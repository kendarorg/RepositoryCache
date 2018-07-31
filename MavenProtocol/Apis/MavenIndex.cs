using System;

namespace MavenProtocol.Apis
{
    public class MavenIndex
    {
        public string[] Group { get; set; }
        public string ArtifactId { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }    //maven-metadata/jar/pom
        public string SubType { get; set; } //null/asc/sha1/md5

        public string ToLocalPath()
        {
            return string.Join("\\", string.Join("\\", Group), ArtifactId, ArtifactId + "-" + Version + "." + Type);
        }
    }
}
