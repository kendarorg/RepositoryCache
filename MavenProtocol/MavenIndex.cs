using System;

namespace MavenProtocol.Apis
{
    public class MavenIndex
    {
        public string[] Group { get; set; }
        public string ArtifactId { get; set; }
        public string Version { get; set; }
        public string Classifier { get; set; } //-sources/-
        public string Extension { get; set; }    //maven-metadata/jar/pom
        public string Checksum { get; set; } //null/asc/sha1/md5
        public string Filename { get; set; }
        public bool IsSnapshot { get; set; }
        public string Meta { get; set; }
        public string Build { get; set; }
        public Guid RepoId { get; set; }
        public byte[] Content { get; set; }
        public DateTime Timestamp { get; set; }

        public string ToLocalPath()
        {
            var snapshot = IsSnapshot ? "-SNAPSHOT" : "";
            var classifier = string.IsNullOrWhiteSpace(Classifier) ? "" : "-" + Classifier;
            return string.Join("\\", string.Join("\\", Group), ArtifactId, ArtifactId + "-" + Version + snapshot
                + classifier + "." + Extension);
        }
    }
}
