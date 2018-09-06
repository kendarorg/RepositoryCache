using Repositories;

namespace Maven.News
{
    public class MetadataEntity : BaseEntity
    {
        public bool Initialized { get; set; }
        public string Group { get; set; }
        public string ArtifactId { get; set; }
        public string Xml { get; set; }
        public string Sha1 { get; set; }
        public string Md5 { get; set; }
        public bool IsSnapshot { get; set; }
        public string Version { get; set; }
        public int Timestamp { get; internal set; }
    }
}