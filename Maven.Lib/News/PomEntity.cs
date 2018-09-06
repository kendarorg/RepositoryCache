using System;
using Repositories;

namespace Maven.News
{
    public class PomEntity : BaseEntity
    {
        internal string OriginalXml;

        public string ArtifactId { get; set; }
        public string Group { get; set; }
        public string Version { get; set; }
        public bool IsSnapshot { get; set; }
        public string Build { get; set; }
        public DateTime Timestamp { get; set; }
        public string Xml { get; set; }
        public object Md5 { get; internal set; }
        public object Sha1 { get; internal set; }
    }
}