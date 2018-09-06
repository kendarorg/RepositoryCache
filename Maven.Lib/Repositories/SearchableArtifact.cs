using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public class SearchableArtifact : BaseEntity
    {
        public string Checksums { get; set; }
        public string ArtifactId { get; set; }
        public string Version { get; set; }
        public string Group { get; set; }
        public string Pom { get; set; }
        public string PomChecksums { get; set; }
        public string Classifiers { get; set; }
        public string Packaging { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid OwnerMetadataId { get; set; }
        public string BuildNumber { get; set; }
        public bool IsSnapshot { get; set; }
        public string Tags { get; set; }
        public Guid RepositoryId { get; set; }
        public String FreeText
        {
            get
            {
                return ArtifactId + Version + Group + Pom + Classifiers + Packaging + Tags;
            }
        }
    }
}
