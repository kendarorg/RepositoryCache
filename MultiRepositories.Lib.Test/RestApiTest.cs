using System;
using NUnit.Framework;

namespace MultiRepositories
{
    [TestFixture]
    public class RestApiTest
    {
        [Test]
        public void ISBPToHandleSimpleUrls()
        {
            var viewed = false;

            var mockRest = new MockRestApi((a) =>
            {
                viewed = true;
                return new SerializableResponse();
            }, "test");

            var url = "test";
            var canSee = mockRest.CanHandleRequest(url);
            Assert.IsTrue(canSee);
            var req = new SerializableRequest()
            {
                Url = url
            };
            var result = mockRest.HandleRequest(req);
            Assert.IsTrue(viewed);
            Assert.IsNotNull(result);
        }


        [Test]
        public void ISBPToHandleSimpleReplaceUrls()
        {
            var viewed = false;

            var mockRest = new MockRestApi((a) =>
            {
                viewed = true;
                return new SerializableResponse();
            }, "test/{first}/index.json");

            var url = "test/data/index.json";
            var canSee = mockRest.CanHandleRequest(url);
            Assert.IsTrue(canSee);
            var req = new SerializableRequest()
            {
                Url = url
            };
            var result = mockRest.HandleRequest(req);
            Assert.IsTrue(req.PathParams.ContainsKey("first"));
            Assert.AreEqual("data", req.PathParams["first"]);
            Assert.IsTrue(viewed);
            Assert.IsNotNull(result);
        }


        [Test]
        public void ISBPToHandleMixedReplaceUrls()
        {
            var viewed = false;

            var mockRest = new MockRestApi((a) =>
            {
                viewed = true;
                return new SerializableResponse();
            }, "test/a{first}b/index.json");

            var url = "test/adatab/index.json";
            var canSee = mockRest.CanHandleRequest(url);
            Assert.IsTrue(canSee);
            var req = new SerializableRequest()
            {
                Url = url
            };
            var result = mockRest.HandleRequest(req);
            Assert.IsTrue(req.PathParams.ContainsKey("first"));
            Assert.AreEqual("data", req.PathParams["first"]);
            Assert.IsTrue(viewed);
            Assert.IsNotNull(result);
        }


        [Test]
        public void ISBPToHandleStarUrls()
        {
            var viewed = false;

            var mockRest = new MockRestApi((a) =>
            {
                viewed = true;
                return new SerializableResponse();
            }, "test/{*first}/index.json");

            var url = "test/test/item/index.json";
            var canSee = mockRest.CanHandleRequest(url);
            Assert.IsTrue(canSee);
            var req = new SerializableRequest()
            {
                Url = url
            };
            var result = mockRest.HandleRequest(req);
            Assert.IsTrue(req.PathParams.ContainsKey("first"));
            Assert.AreEqual("test/item", req.PathParams["first"]);
            Assert.IsTrue(viewed);
            Assert.IsNotNull(result);
        }

        [Test]
        public void ISBPToThrowOnduplicateStars()
        {

            var mockRest = new MockRestApi((a) => null, "test/{*first}/{*last}.json");

            var url = "test/test/item/index.json";
            Assert.Throws<Exception>(() => mockRest.CanHandleRequest(url));
        }


        [Test]
        public void ISBPToHandleStarUrlsEnding()
        {
            var viewed = false;

            var mockRest = new MockRestApi((a) =>
            {
                viewed = true;
                return new SerializableResponse();
            }, "test/{*first}");

            var url = "test/test/item/index.json";
            var canSee = mockRest.CanHandleRequest(url);
            Assert.IsTrue(canSee);
            var req = new SerializableRequest()
            {
                Url = url
            };
            var result = mockRest.HandleRequest(req);
            Assert.IsTrue(req.PathParams.ContainsKey("first"));
            Assert.AreEqual("test/item/index.json", req.PathParams["first"]);
            Assert.IsTrue(viewed);
            Assert.IsNotNull(result);
        }


        [Test]
        public void ISBPToHandleStarUrlsStarting()
        {
            var viewed = false;

            var mockRest = new MockRestApi((a) =>
            {
                viewed = true;
                return new SerializableResponse();
            }, "{*first}/test");

            var url = "test/cicca/test";
            var canSee = mockRest.CanHandleRequest(url);
            Assert.IsTrue(canSee);
            var req = new SerializableRequest()
            {
                Url = url
            };
            var result = mockRest.HandleRequest(req);
            Assert.IsTrue(req.PathParams.ContainsKey("first"));
            Assert.AreEqual("test/cicca", req.PathParams["first"]);
            Assert.IsTrue(viewed);
            Assert.IsNotNull(result);
        }



        [Test]
        public void ISBPShouldNotHandleMissing()
        {
            var mockRest = new MockRestApi((a) => null, "test/prova.json");

            var url = "test/prova.json/fuffa";
            var result = mockRest.CanHandleRequest(url);
            Assert.IsFalse(result);
        }

        [Test]
        public void ISBPToHandleSimpleUrlWithMethod()
        {
            var viewed = false;

            var mockRest = new MockRestApi((a) =>
            {
                viewed = true;
                return new SerializableResponse();
            }, "*PUT", "test");

            var url = "test";
            var canSee = mockRest.CanHandleRequest(url, "PUT");
            Assert.IsTrue(canSee);
            var req = new SerializableRequest()
            {
                Url = url,
                Method = "PUT"
            };
            var result = mockRest.HandleRequest(req);
            Assert.IsTrue(viewed);
            Assert.IsNotNull(result);
        }


        [Test]
        public void ISBPToIgnoreNotMatchingMethodInCanHandle()
        {
            var mockRest = new MockRestApi((a) =>
            {
                return new SerializableResponse();
            }, "*PUT", "test");

            var url = "test";
            var canSee = mockRest.CanHandleRequest(url, "POST");
            Assert.IsFalse(canSee);
        }


        [Test]
        public void ISBPToIgnoreNotMatchingMethodInHandleRequest()
        {
            var mockRest = new MockRestApi((a) =>
            {
                return new SerializableResponse();
            }, "*PUT", "test");

            var url = "test";
            var req = new SerializableRequest()
            {
                Url = url,
                Method = "POST"
            };
            Assert.Throws<Exception>(() => mockRest.HandleRequest(req));
        }


        [Test]
        public void ISBPToParseCorrectlyMavenUrl()
        {
            var mockRest = new MockRestApi((a) =>
            {
                return new SerializableResponse();
            }, "/{repo}/{*group}/{id}/maven-metadata.xml");
            
            Assert.IsFalse(mockRest.CanHandleRequest("/nuget.org/v3/index.json"));
            Assert.IsTrue(mockRest.CanHandleRequest("/maven.local/org/slf4j/slf4j-api/maven-metadata.xml"));
        }


        [Test]
        public void ISBPToMatchRegex()
        {
            SerializableRequest request = null;
            var mockRest = new MockRestApi((a) =>
            {
                request = a;
                return new SerializableResponse();
            }, @"/{repo}/{id#^(?<major>\d+)$}/test");

            var url = "/nuget.org/33/test";
            Assert.IsTrue(mockRest.CanHandleRequest(url));
            var req = new SerializableRequest()
            {
                Url = url,
                Method = "POST"
            };
            mockRest.HandleRequest(req);
            Assert.IsNotNull(request);
            Assert.AreEqual("33", request.PathParams["major"]);
            Assert.AreEqual("33", request.PathParams["id"]);
        }


        [Test]
        public void ISBPToMatchRegexMultiple()
        {
            SerializableRequest request = null;
            var mockRest = new MockRestApi((a) =>
            {
                request = a;
                return new SerializableResponse();
            }, @"/{repo}/{id#^(?<major>\d+)(\.(?<minor>\d+))?$}/test");

            var url = "/nuget.org/33.22/test";
            Assert.IsTrue(mockRest.CanHandleRequest(url));
            var req = new SerializableRequest()
            {
                Url = url,
                Method = "POST"
            };
            mockRest.HandleRequest(req);
            Assert.IsNotNull(request);
            Assert.AreEqual("33", request.PathParams["major"]);
            Assert.AreEqual("22", request.PathParams["minor"]);
            Assert.AreEqual("33.22", request.PathParams["id"]);
        }
    }
}
