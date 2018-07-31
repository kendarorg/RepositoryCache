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
    }

    [XmlRoot(ElementName = "metadata")]
    public class MavenMetadata
    {
        [XmlElement(ElementName = "groupId")]
        public string GroupId { get; set; }
        [XmlElement(ElementName = "artifactId")]
        public string ArtifactId { get; set; }
        [XmlElement(ElementName = "versioning")]
        public MavenVersioning Versioning { get; set; }
        [XmlAttribute(AttributeName = "modelVersion")]
        public string ModelVersion { get; set; }
    }
}
