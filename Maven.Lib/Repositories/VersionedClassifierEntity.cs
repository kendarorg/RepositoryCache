using System;
using Repositories;

namespace Maven.Repositories
{
    public class VersionedClassifierEntity:BaseEntity
    {
        public string Checksums { get; set; }
        public string ArtifactId { get; set; }
        public bool IsSnapshot { get; set; }
        public string Version { get; set; }
        public string Group { get; set; }
        public string Classifer { get; set; }
        public Guid OwnerArtifactId { get; set; }
        public string Packaging { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid OwnerMetadataId { get; set; }
    }
}