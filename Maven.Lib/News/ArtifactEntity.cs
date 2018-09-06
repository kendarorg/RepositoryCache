using Repositories;
using System;

namespace Maven.News
{
    public class ArtifactEntity : BaseEntity
    {
        public string Classifier { get; set; }
        public string Extension { get; set; }
        public string Version { get; set; }
        public DateTime Timestamp { get; set; }
        public string Build { get; set; }
        public string ArtifactId { get; internal set; }
        public bool IsSnapshot { get; internal set; }
        public string Group { get; internal set; }
        public string Md5 { get; internal set; }
        public string Sha1 { get; internal set; }
    }
}