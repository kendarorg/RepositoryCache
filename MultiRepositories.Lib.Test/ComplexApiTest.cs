using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MultiRepositories
{
    [TestClass] 
    public class ComplexApiTest
    {


        const string FULL_REGEX = @"/{repo}/{*path}/" +
                @"{pack#^(?<package>[0-9A-Za-z\-\.]+)$}/" +
                @"{ver#^(?<major>\d+)" +
                @"(\.(?<minor>\d+))?" +
                @"(\.(?<patch>\d+))?" +
                @"(\.(?<extra>\d+))?" +
                @"(\-(?<pre>[0-9A-Za-z\.]+))?" +
                @"(\-(?<build>[0-9A-Za-z\-\.]+))?$}/" +
                @"{filename#^(?<fullpackage>(?:(?!\b(?:jar|pom)\b)[0-9A-Za-z\-\.])+)?" +
                @"\.(?<type>(jar|pom))" +
                @"(\.(?<subtype>(asc|md5|sha1)))?$}";


        const string METADATA_REGEX = @"/{repo}/{*path}/" +
                @"{pack#^(?<package>[0-9A-Za-z\-\.]+)$}/" +
                @"{meta#^(?<filename>(maven-metadata.xml))(\.(?<subtype>(asc|md5|sha1)))?$}";

        [TestMethod]
        public void ISBPToMatchRegexJarMd5Comp()
        {
            SerializableRequest request = null;
            var mockRest = new MockRestApi((a) =>
            {
                request = a;
                return new SerializableResponse();
            }, FULL_REGEX);
            
            var url = "/maven.local/org/slf4j/slf4j-api/1.7.25/slf4j-api-1.7.25.jar.md5";

            Assert.IsFalse(mockRest.CanHandleRequest(
                "/maven.local/org/slf4j/slf4j-api/a1.7.25/slf4j-api-1.7.25.jar.md5"));
            Assert.IsFalse(mockRest.CanHandleRequest(
                "/maven.local/org/slf4j/slf4j-api/maven-metadata.xml"));
            Assert.IsTrue(mockRest.CanHandleRequest(
                "/maven.local/org/slf4j/slf4j-api/1.7.25-a/slf4j-api-1.7.25.jar.md5"));
            Assert.IsTrue(mockRest.CanHandleRequest(url));
            var req = new SerializableRequest() {Url = url};
            mockRest.HandleRequest(req);
            Assert.IsNotNull(request);

            Assert.AreEqual("1", request.PathParams["major"]);
            Assert.AreEqual("7", request.PathParams["minor"]);
            Assert.AreEqual("25", request.PathParams["patch"]);
            Assert.AreEqual("slf4j-api", request.PathParams["package"]);
            Assert.AreEqual("org/slf4j", request.PathParams["*path"]);
            Assert.AreEqual("slf4j-api-1.7.25", request.PathParams["fullpackage"]);
            Assert.AreEqual("jar", request.PathParams["type"]);
            Assert.AreEqual("md5", request.PathParams["subtype"]);
        }


        [TestMethod]
        public void ISBPToMatchRegexJarComp()
        {
            SerializableRequest request = null;
            var mockRest = new MockRestApi((a) =>
            {
                request = a;
                return new SerializableResponse();
            }, FULL_REGEX);

            var url = "/maven.local/org/slf4j/slf4j-api/1.7.25/slf4j-api-1.7.25.jar";

            Assert.IsTrue(mockRest.CanHandleRequest(url));
            var req = new SerializableRequest() { Url = url };
            mockRest.HandleRequest(req);
            Assert.IsNotNull(request);

            Assert.AreEqual("1", request.PathParams["major"]);
            Assert.AreEqual("7", request.PathParams["minor"]);
            Assert.AreEqual("25", request.PathParams["patch"]);
            Assert.AreEqual("slf4j-api", request.PathParams["package"]);
            Assert.AreEqual("org/slf4j", request.PathParams["*path"]);
            Assert.AreEqual("slf4j-api-1.7.25", request.PathParams["fullpackage"]);
            Assert.AreEqual("jar", request.PathParams["type"]);
            Assert.IsFalse(request.PathParams.ContainsKey("subtype"));
        }


        [TestMethod]
        public void ISBPToMatchRegexPomComp()
        {
            SerializableRequest request = null;
            var mockRest = new MockRestApi((a) =>
            {
                request = a;
                return new SerializableResponse();
            }, FULL_REGEX);

            var url = "/maven.local/org/slf4j/slf4j-api/1.7.25/slf4j-api-1.7.25.pom";

            Assert.IsTrue(mockRest.CanHandleRequest(url));
            var req = new SerializableRequest() { Url = url };
            mockRest.HandleRequest(req);
            Assert.IsNotNull(request);

            Assert.AreEqual("1", request.PathParams["major"]);
            Assert.AreEqual("7", request.PathParams["minor"]);
            Assert.AreEqual("25", request.PathParams["patch"]);
            Assert.AreEqual("slf4j-api", request.PathParams["package"]);
            Assert.AreEqual("org/slf4j", request.PathParams["*path"]);
            Assert.AreEqual("slf4j-api-1.7.25", request.PathParams["fullpackage"]);
            Assert.AreEqual("pom", request.PathParams["type"]);
            Assert.IsFalse(request.PathParams.ContainsKey("subtype"));
        }


        [TestMethod]
        public void ISBPToMatchRegexPomAscComp()
        {
            SerializableRequest request = null;
            var mockRest = new MockRestApi((a) =>
            {
                request = a;
                return new SerializableResponse();
            }, FULL_REGEX);

            var url = "/maven.local/org/slf4j/slf4j-api/1.7.25/slf4j-api-1.7.25.pom.asc";

            Assert.IsTrue(mockRest.CanHandleRequest(url));
            var req = new SerializableRequest() { Url = url };
            mockRest.HandleRequest(req);
            Assert.IsNotNull(request);

            Assert.AreEqual("1", request.PathParams["major"]);
            Assert.AreEqual("7", request.PathParams["minor"]);
            Assert.AreEqual("25", request.PathParams["patch"]);
            Assert.AreEqual("slf4j-api", request.PathParams["package"]);
            Assert.AreEqual("org/slf4j", request.PathParams["*path"]);
            Assert.AreEqual("slf4j-api-1.7.25", request.PathParams["fullpackage"]);
            Assert.AreEqual("pom", request.PathParams["type"]);
            Assert.AreEqual("asc", request.PathParams["subtype"]);
        }


        [TestMethod]
        public void ISBPToMatchRegexmetadataAscComp()
        {
            SerializableRequest request = null;
            var mockRest = new MockRestApi((a) =>
            {
                request = a;
                return new SerializableResponse();
            }, METADATA_REGEX);

            var url = "/maven.local/org/slf4j/slf4j-api/maven-metadata.xml.asc";

            Assert.IsTrue(mockRest.CanHandleRequest(url));
            var req = new SerializableRequest() { Url = url };
            mockRest.HandleRequest(req);
            Assert.IsNotNull(request);

            Assert.AreEqual("slf4j-api", request.PathParams["package"]);
            Assert.AreEqual("org/slf4j", request.PathParams["*path"]);
            Assert.AreEqual("maven-metadata.xml", request.PathParams["filename"]);
            Assert.AreEqual("asc", request.PathParams["subtype"]);
        }



        [TestMethod]
        public void ISBPToMatchRegexmetadataComp()
        {
            SerializableRequest request = null;
            var mockRest = new MockRestApi((a) =>
            {
                request = a;
                return new SerializableResponse();
            }, METADATA_REGEX);

            var url = "/maven.local/org/slf4j/slf4j-api/maven-metadata.xml";

            Assert.IsTrue(mockRest.CanHandleRequest(url));
            var req = new SerializableRequest() { Url = url };
            mockRest.HandleRequest(req);
            Assert.IsNotNull(request);

            Assert.AreEqual("slf4j-api", request.PathParams["package"]);
            Assert.AreEqual("org/slf4j", request.PathParams["*path"]);
            Assert.AreEqual("maven-metadata.xml", request.PathParams["filename"]);
            Assert.IsFalse(request.PathParams.ContainsKey("subtype"));
        }
    }
}
