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

        public string ToLocalPath(bool hasTimestampedBuild)
        {
            var snapshot = IsSnapshot ? "-SNAPSHOT" : "";
            var snapshotBuild = snapshot;
            if(IsSnapshot && hasTimestampedBuild)
            {
                snapshotBuild = "-" + Timestamp.ToString("yyyyMMdd.HHmmss") + "-" + Build;
            }
            var classifier = string.IsNullOrWhiteSpace(Classifier) ? "" : "-" + Classifier;
            return string.Join("\\", string.Join("\\", Group), ArtifactId, Version + snapshot, 
                ArtifactId + "-" + Version + snapshotBuild
                + classifier + "." + Extension);
        }
    }
}
