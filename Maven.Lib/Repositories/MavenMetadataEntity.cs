using System;
using Repositories;

namespace Maven.Repositories
{
    public class MavenMetadataEntity : BaseEntity
    {
        public Guid RepositoryId { get; set; }
        public string Checksums { get; set; }
        public string ArtifactId { get; set; }
        public string Group { get; set; }
        public string Xml { get; set; }
        public string Latest { get; set; }
        public string Release { get; set; }
        public string Snapshot { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool IsSnapshot { get; set; }
        public string Version { get; set; }
        public string BuildNumber { get; set; }
    }
}