using System;
using Repositories;

namespace Maven.News
{
    public class PomEntity : BaseEntity
    {
        public string OriginalXml { get; set; }

        public string ArtifactId { get; set; }
        public string Group { get; set; }
        public string Version { get; set; }
        public bool IsSnapshot { get; set; }
        public string Build { get; set; }
        public DateTime Timestamp { get; set; }
        public string Xml { get; set; }
        public string Md5 { get;  set; }
        public string Sha1 { get;  set; }
        public Guid RepositoryId { get; set; }
        public string Packaging { get; set; }
        public string Classifiers { get; set; }
        public string Tags { get; set; }
        public string FreeText { get; set; }
    }
}