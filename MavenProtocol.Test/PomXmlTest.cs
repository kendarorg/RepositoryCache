using System;
using System.Text;
using Ioc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nuget.Lib.Test.Utils;

namespace MavenProtocol.Test
{
    [TestClass]
    public class PomXmlTest
    {
        private AssemblyUtils _au = new AssemblyUtils();

        [TestMethod]
        public void ITSPToParseSimplePom()
        {
            var pom = _au.ReadRes<JsonComp>("simplepom.xml");
            var target = PomXml.Parse(pom);

            JsonComp.Equals("ITSPToParseSimplePom.json", target);
        }

        [TestMethod]
        public void ITSPToParseRealPom()
        {
            var pom = _au.ReadRes<JsonComp>("slf4j-api-1.7.25.pom");
            var target = PomXml.Parse(pom);

            JsonComp.Equals("ITSPToParseRealPom.json", target);
        }
    }
}
