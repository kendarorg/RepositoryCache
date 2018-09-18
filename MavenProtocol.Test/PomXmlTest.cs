using System;
using System.Text;
using Ioc;
using NUnit.Framework;
using Nuget.Lib.Test.Utils;

namespace MavenProtocol.Test
{
    [TestFixture]
    public class PomXmlTest
    {
        private AssemblyUtils _au = new AssemblyUtils();

        [Test]
        public void ITSPToParseSimplePom()
        {
            var pom = _au.ReadRes<JsonComp>("simplepom.xml");
            var target = PomXml.Parse(pom);

            JsonComp.Equals("ITSPToParseSimplePom.json", target);
        }

        [Test]
        public void ITSPToParseRealPom()
        {
            var pom = _au.ReadRes<JsonComp>("slf4j-api-1.7.25.pom");
            var target = PomXml.Parse(pom);

            JsonComp.Equals("ITSPToParseRealPom.json", target);
        }
    }
}
