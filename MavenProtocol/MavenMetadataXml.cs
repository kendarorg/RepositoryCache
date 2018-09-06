using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace MavenProtocol
{
    [XmlType( "versions")]
    public class MavenVersions
    {
        [XmlElement(ElementName = "version")]
        public List<string> Version { get; set; }
    }

    [XmlType( "versioning")]
    public class MavenVersioning
    {
        [XmlElement(ElementName = "latest")]
        public string Latest { get; set; }
        [XmlElement(ElementName = "release")]
        public string Release { get; set; }
        [XmlElement(ElementName = "versions")]
        public MavenVersions Versions { get; set; }
        [XmlElement(ElementName = "lastUpdated")]
        public string LastUpdated { get; set; }
        [XmlElement(ElementName = "snapshotVersions")]
        public MavenSnapshotVersions SnapshotVersions { get; set; }
        [XmlElement(ElementName ="snapshot")]
        public MavenSnapshot Snapshot { get; set; }
    }

    [XmlType("snapshotVersions")]
    public class MavenSnapshotVersions
    {
        [XmlElement(ElementName = "snapshotVersion")]
        public List<MavenSnapshotVersion> Version { get; set; }
    }

    [XmlType("snapshotVersion")]
    public class MavenSnapshotVersion
    {
        [XmlElement(ElementName = "classifier")]
        public string Classifier { get; set; }
        [XmlElement(ElementName = "extension")]
        public string Extension { get; set; }
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "updated")]
        public string Updated { get; set; }
    }

    [XmlType("snapshot")]
    public class MavenSnapshot
    {
        [XmlElement(ElementName = "timestamp")]
        public string Timestamp { get; set; }
        [XmlElement(ElementName = "buildNumber")]
        public string BuildNumber { get; set; }
        [XmlElement(ElementName = "localCopy")]
        public string LocalCopy { get; set; }
    }

    [XmlType("plugin")]
    public class MavenPlugin
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "prefix")]
        public string Prefix { get; set; }
        [XmlElement(ElementName = "artifactId")]
        public string ArtifactId { get; set; }
    }

    [XmlType("plugins")]
    public class MavenPlugins
    {
        [XmlElement(ElementName = "plugin")]
        public List<MavenPlugin> Plugin { get; set; }
    }

    [XmlRoot(ElementName = "metadata")]
    public class MavenMetadataXml
    {
        public MavenMetadataXml()
        {
            ModelVersion = "4.0.0";
        }
        [XmlElement(ElementName = "groupId")]
        public string GroupId { get; set; }
        [XmlElement(ElementName = "artifactId")]
        public string ArtifactId { get; set; }
        [XmlElement(ElementName = "versioning")]
        public MavenVersioning Versioning { get; set; }
        [XmlElement(ElementName = "version")]
        public String Version { get; set; }
        [XmlAttribute(AttributeName = "modelVersion")]
        public string ModelVersion { get; set; }
        [XmlElement(ElementName = "plugins")]
        public MavenPlugins Plugins { get; set; }
    }
}
