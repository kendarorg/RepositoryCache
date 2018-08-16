using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public abstract class MavenBaseSearchEntity : BaseEntity
    {
        public Guid RepositoryId { get; set; }
        public string FreeText { get; set; }
        public string ArtifactId { get; set; }
        public string Group { get; set; }
        public string Version { get; set; }
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Should exist one for classifier
        /// </summary>
        public string Type { get; set; } //jar pom
        /// <summary>
        /// Default should be NULLfor teh real jar
        /// </summary>
        public string Classifiers { get; set; } //null/-sources.jar etc |xx|dd|
        public string Tags { get; set; } //|xx|dd|

    }
}
