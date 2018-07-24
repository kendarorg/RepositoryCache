/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace NugetProtocol
{
    [XmlType("repository")]
    public class RepositoryXml
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "url")]
        public string Url { get; set; }
    }

    [XmlType("group")]
    public class GroupXml
    {
        [XmlAttribute(AttributeName = "targetFramework")]
        public string TargetFramework { get; set; }
        [XmlElement(ElementName = "dependency")]
        public List<DependencyXml> Dependency { get; set; }
    }

    [XmlType("dependency")]
    public class DependencyXml
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "exclude")]
        public string Exclude { get; set; }
    }

    [XmlType("dependencies")]
    public class DependenciesXml
    {
        [XmlElement(ElementName = "group")]
        public List<GroupXml> Group { get; set; }

        [XmlElement(ElementName = "dependency")]
        public List<DependencyXml> Dependency { get; set; }
    }

    [XmlType("frameworkAssembly")]
    public class FrameworkAssemblyXml
    {
        [XmlAttribute(AttributeName = "assemblyName")]
        public string AssemblyName { get; set; }
        [XmlAttribute(AttributeName = "targetFramework")]
        public string TargetFramework { get; set; }
    }

    [XmlType("frameworkAssemblies")]
    public class FrameworkAssembliesXml
    {
        [XmlElement(ElementName = "frameworkAssembly")]
        public List<FrameworkAssemblyXml> FrameworkAssembly { get; set; }
    }

    [XmlType("metadata")]
    public class MetadataXml
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "version")]
        public string Version { get; set; }
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "authors")]
        public string Authors { get; set; }
        [XmlElement(ElementName = "owners")]
        public string Owners { get; set; }
        [XmlElement(ElementName = "requireLicenseAcceptance")]
        public string RequireLicenseAcceptance { get; set; }
        [XmlElement(ElementName = "licenseUrl")]
        public string LicenseUrl { get; set; }
        [XmlElement(ElementName = "projectUrl")]
        public string ProjectUrl { get; set; }
        [XmlElement(ElementName = "iconUrl")]
        public string IconUrl { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "summary")]
        public string Summary { get; set; }
        [XmlElement(ElementName = "releaseNotes")]
        public string ReleaseNotes { get; set; }
        [XmlElement(ElementName = "copyright")]
        public string Copyright { get; set; }
        [XmlElement(ElementName = "serviceable")]
        public string Serviceable { get; set; }
        [XmlElement(ElementName = "language")]
        public string Language { get; set; }
        [XmlElement(ElementName = "tags")]
        public string Tags { get; set; }
        [XmlElement(ElementName = "repository")]
        public RepositoryXml Repository { get; set; }
        [XmlElement(ElementName = "dependencies")]
        public DependenciesXml Dependencies { get; set; }
        [XmlElement(ElementName = "frameworkAssemblies")]
        public FrameworkAssembliesXml FrameworkAssemblies { get; set; }
        [XmlAttribute(AttributeName = "minClientVersion")]
        public string MinClientVersion { get; set; }
    }

    [XmlRoot(ElementName = "package")]
    public class PackageXml
    {
        [XmlElement(ElementName = "metadata")]
        public MetadataXml Metadata { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

}
