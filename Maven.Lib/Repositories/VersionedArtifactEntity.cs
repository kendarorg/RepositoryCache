using System;
using Repositories;

namespace Maven.Repositories
{
    public class VersionedArtifactEntity : BaseEntity
    {
        public string Checksums { get; set; }
        public string PomChecksums { get; set; }
        public string ArtifactId { get; set; }
        public string Version { get; set; }
        public bool IsSnapshot { get; set; }
        public string Group { get; set; }
        public string Pom { get; set; }
        public string Classifiers { get; set; }
        public string Packaging { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid OwnerMetadataId { get; set; }
        public string BuildNumber { get; set; }
        public string Tags { get; internal set; }
    }
}