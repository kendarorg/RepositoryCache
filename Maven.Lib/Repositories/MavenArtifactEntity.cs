using Repositories;
using System;

namespace Maven.Repositories
{
    public class MavenArtifactEntity : BaseEntity
    {
        public string ArtifactId { get; set; }
        public string Version { get; set; }
        public string Classifier { get; set; }
        public string Type { get; set; }
        public string Md5 { get; set; }
        public string Sha1 { get; set; }
        public string Asc { get; set; }
        public DateTime Timestamp { get; set; }
    }
}