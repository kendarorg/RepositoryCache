using Repositories;
using System;

namespace Maven.News
{
    public class ArtifactVersion : BaseEntity
    {
        public DateTime Timestamp { get; set; }
        public bool IsSnapshot { get;  set; }
        public string Version { get; set; }
        public string Build { get; set; }
        public string ArtifactId { get; set; }
        public string Group { get; set; }
        public string Sha1 { get; set; }
        public string Md5 { get; set; }
        public string Classifier { get; set; }
    }
}