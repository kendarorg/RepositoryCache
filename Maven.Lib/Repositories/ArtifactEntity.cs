using System;
using Repositories;

namespace Maven.Repositories
{
    public class ArtifactEntity : BaseEntity
    {
        public string Checksums { get; set; }
        public string ArtifactId { get; set; }
        public string Group { get; set; }
        public string Xml { get; set; }
        public string Latest { get; set; }
        public string Release { get; set; }
        public string Snapshot { get; set; }
        public DateTime LastUpdated { get; set; }
        public Guid RepositoryId { get; set; }
        public string JsonPlugins { get; internal set; }
        public string Version { get; internal set; }
        public bool IsSnapshot { get; internal set; }
    }
}