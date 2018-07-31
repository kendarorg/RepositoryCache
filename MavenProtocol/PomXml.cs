using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MavenProtocol
{/*
        <licenses>
      <license>
        <name>Apache License, Version 2.0</name>
        <url>https://www.apache.org/licenses/LICENSE-2.0.txt</url>
        <distribution>repo</distribution>
        <comments>A business-friendly OSS license</comments>
      </license>
    </licenses>
      <organization>
    <name>Codehaus Mojo</name>
    <url>http://mojo.codehaus.org</url>
  </organization>
    <developers>
    <developer>
      <id>jdoe</id>
      <name>John Doe</name>
      <email>jdoe@example.com</email>
      <url>http://www.example.com/jdoe</url>
      <organization>ACME</organization>
      <organizationUrl>http://www.example.com</organizationUrl>
      <roles>
        <role>architect</role>
        <role>developer</role>
      </roles>
      <timezone>America/New_York</timezone>
      <properties>
        <picUrl>http://www.example.com/jdoe/pic</picUrl>
      </properties>
    </developer>
  </developers>
    <contributors>
    <contributor>
      <name>Noelle</name>
      <email>some.name@gmail.com</email>
      <url>http://noellemarie.com</url>
      <organization>Noelle Marie</organization>
      <organizationUrl>http://noellemarie.com</organizationUrl>
      <roles>
        <role>tester</role>
      </roles>
      <timezone>America/Vancouver</timezone>
      <properties>
        <gtalk>some.name@gmail.com</gtalk>
      </properties>
    </contributor>
  </contributors>
  <mailingLists>
    <mailingList>
      <name>User List</name>
      <subscribe>user-subscribe@127.0.0.1</subscribe>
      <unsubscribe>user-unsubscribe@127.0.0.1</unsubscribe>
      <post>user@127.0.0.1</post>
      <archive>http://127.0.0.1/user/</archive>
      <otherArchives>
        <otherArchive>http://base.google.com/base/1/127.0.0.1</otherArchive>
      </otherArchives>
    </mailingList>
  </mailingLists>
  <issueManagement>
    <system>Bugzilla</system>
    <url>http://127.0.0.1/bugzilla/</url>
  </issueManagement>
    <scm>
    <connection>scm:svn:http://127.0.0.1/svn/my-project</connection>
    <developerConnection>scm:svn:https://127.0.0.1/svn/my-project</developerConnection>
    <tag>HEAD</tag>
    <url>http://127.0.0.1/websvn/my-project</url>
  </scm>
    */
    public class PomXml
    {
        [JsonProperty("groupId")]
        public string GroupId { get; set; }
        [JsonProperty("artifactId")]
        public string ArtifactId { get; set; }
        [JsonProperty("version", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Version { get; set; }
        [JsonProperty("packaging", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Packaging { get; set; }


        [JsonProperty("dependencies", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<DependencyXml> Dependencies { get; set; }
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }
        [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Url { get; set; }
        [JsonProperty("inceptionYear", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string InceptionYear { get; set; }
        [JsonProperty("modules", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Modules { get; set; }

        public static PomXml Parse(string data)
        {
            var result = new PomXml();
            //var indexOf = data.IndexOf("<project", StringComparison.InvariantCultureIgnoreCase);
            //data = data.Substring(indexOf);
            var xml = XElement.Parse(data);

            var parentEl = ChildByName(xml, "parent");
            result.GroupId = ValueByName(xml, "groupId");
            result.ArtifactId = ValueByName(xml, "artifactId");
            result.Version = ValueByName(xml, "version");
            if (result.Version == null)
            {
                result.Version = ValueByName(parentEl, "version");
            }
            if (result.GroupId == null)
            {
                result.GroupId = ValueByName(parentEl, "groupId");
            }
            var packagingEl = ChildByName(xml, "packaging");
            var dependenciesEl = ChildByName(xml, "dependencies");
            if (dependenciesEl != null && dependenciesEl.Elements().Any())
            {
                result.Dependencies = new List<DependencyXml>();
                foreach (var dep in ChildrenByName(dependenciesEl, "dependency"))
                {
                    var depx = new DependencyXml
                    {
                        GroupId = ValueByName(dep, "groupId"),
                        ArtifactId = ValueByName(dep, "artifactId"),
                        Version = ValueByName(dep, "version")
                    };
                    result.Dependencies.Add(depx);
                }
            }

            var modulesEl = ChildByName(xml, "modules");
            if (modulesEl != null && modulesEl.Elements().Any())
            {
                result.Modules = new List<string>();
                foreach (var mod in ChildrenByName(modulesEl, "module"))
                {
                    result.Modules.Add(mod.Value);
                }
            }

            result.Name = ValueByName(xml, "Name");
            result.Description = ValueByName(xml, "description");
            result.Url = ValueByName(xml, "url");
            result.InceptionYear = ValueByName(xml, "InceptionYear");
            return result;
        }
        private static string ValueByName(XElement xml, string group)
        {
            var el = ChildByName(xml, group);
            if (el == null) return null;
            return el.Value;
        }
        private static XElement ChildByName(XElement xml, string group)
        {
            return ChildrenByName(xml, group).FirstOrDefault();
        }
        private static IEnumerable<XElement> ChildrenByName(XElement xml, string group)
        {
            return xml.Elements().Where(e => e.Name.LocalName.ToLowerInvariant() == group.ToLowerInvariant());
        }
    }
}
