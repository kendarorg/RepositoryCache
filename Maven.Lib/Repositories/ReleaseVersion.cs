using Repositories;
using System;

namespace Maven.News
{
    public class ReleaseVersion : BaseEntity
    {
        public DateTime Timestamp { get; set; }
        public bool IsSnapshot { get;  set; }
        public string Version { get; set; }
        public string Build { get; set; }
        public string ArtifactId { get; set; }
        public string Group { get; set; }
    }
}