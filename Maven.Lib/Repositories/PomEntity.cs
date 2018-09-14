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

        public PomEntity Clone(PomEntity release)
        {
            release.ArtifactId = this.ArtifactId;
            release.Build = this.Build;
            release.Classifiers = this.Classifiers;
            release.Group = this.Group;
            if (release.Id == Guid.Empty)
            {
                release.Id = this.Id;
            }
            release.IsSnapshot = this.IsSnapshot;
            release.Md5 = this.Md5;
            release.OriginalXml = this.OriginalXml;
            release.Packaging = this.Packaging;
            release.RepositoryId = this.RepositoryId;
            release.Sha1 = this.Sha1;
            release.Tags = this.Tags;
            release.Timestamp = this.Timestamp;
            release.Version = this.Version;
            release.Xml = this.Xml;
            return release;
        }
    }
}