using System;
using Repositories;

namespace Maven.Repositories
{
    public class MavenSearchEntity : BaseEntity
    {
        public DateTime Timestamp { get; set; }
        public Guid RepositoryId { get; set; }
        public string FreeText { get; set; }
        public string ArtifactId { get; set; }
        public string Group { get; set; }
        public string Version { get; set; }
        public string Type { get; set; } //jar pom
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public int Build { get; set; }
        public string Pre0 { get; set; }
        public string Pre1 { get; set; }
        public string Classifiers { get; set; } //null/-sources.jar etc |xx|dd|
        public string Tags { get;  set; } //|xx|dd|
    }
}