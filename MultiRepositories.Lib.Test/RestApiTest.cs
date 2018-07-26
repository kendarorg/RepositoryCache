using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MultiRepositories
{
    [TestClass]
    public class RestApiTest
    {
        [TestMethod]
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


        [TestMethod]
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


        [TestMethod]
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


        [TestMethod]
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
            Assert.IsTrue(req.PathParams.ContainsKey("*first"));
            Assert.AreEqual("test/item", req.PathParams["*first"]);
            Assert.IsTrue(viewed);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ISBPToThrowOnduplicateStars()
        {
            var viewed = false;

            var mockRest = new MockRestApi((a) =>
            {
                viewed = true;
                return new SerializableResponse();
            }, "test/{*first}/{*last}.json");

            var url = "test/test/item/index.json";
            mockRest.CanHandleRequest(url);
        }


        [TestMethod]
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
            Assert.IsTrue(req.PathParams.ContainsKey("*first"));
            Assert.AreEqual("test/item/index.json", req.PathParams["*first"]);
            Assert.IsTrue(viewed);
            Assert.IsNotNull(result);
        }


        [TestMethod]
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
            Assert.IsTrue(req.PathParams.ContainsKey("*first"));
            Assert.AreEqual("test/cicca", req.PathParams["*first"]);
            Assert.IsTrue(viewed);
            Assert.IsNotNull(result);
        }



        [TestMethod]
        public void ISBPShouldNotHandleMissing()
        {
            var mockRest = new MockRestApi((a) => null, "test/prova.json");

            var url = "test/prova.json/fuffa";
            var result = mockRest.CanHandleRequest(url);
            Assert.IsFalse(result);
        }
    }
}
