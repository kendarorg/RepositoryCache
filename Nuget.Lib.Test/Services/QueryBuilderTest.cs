using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nuget.Services;

namespace NugetProtocol
{
    [TestClass]
    public class QueryBuilderTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var target = new QueryBuilder();
            var result = target.ParseQuery("a");
            
            Assert.AreEqual(0, result.Keys.Count);
            Assert.AreEqual(1, result.FreeText.Count);
            Assert.AreEqual("a", result.FreeText[0]);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var target = new QueryBuilder();
            var result = target.ParseQuery("a+b");

            Assert.AreEqual(0, result.Keys.Count);
            Assert.AreEqual(1, result.FreeText.Count);
            Assert.AreEqual("a b", result.FreeText[0]);
        }

        [TestMethod]
        public void TestMethod3()
        {
            var target = new QueryBuilder();
            var result = target.ParseQuery(@"""a b""");

            Assert.AreEqual(0, result.Keys.Count);
            Assert.AreEqual(1, result.FreeText.Count);
            Assert.AreEqual("a b", result.FreeText[0]);
        }

        [TestMethod]
        public void TestMethod4()
        {
            var target = new QueryBuilder();
            var result = target.ParseQuery(@"""a+b""");

            Assert.AreEqual(0, result.Keys.Count);
            Assert.AreEqual(1, result.FreeText.Count);
            Assert.AreEqual("a+b", result.FreeText[0]);
        }

        [TestMethod]
        public void TestMethod5()
        {
            var target = new QueryBuilder();
            var result = target.ParseQuery(@"id:""a""");

            Assert.AreEqual(1, result.Keys.Count);
            Assert.AreEqual(0, result.FreeText.Count);
            Assert.IsTrue(result.Keys.ContainsKey("id"));
            Assert.AreEqual("a", result.Keys["id"]);
        }

        [TestMethod]
        public void TestMethod6()
        {
            var target = new QueryBuilder();
            var result = target.ParseQuery(@"id:""a+b""");

            Assert.AreEqual(1, result.Keys.Count);
            Assert.AreEqual(0, result.FreeText.Count);
            Assert.IsTrue(result.Keys.ContainsKey("id"));
            Assert.AreEqual("a+b", result.Keys["id"]);
        }

        [TestMethod]
        public void TestMethod7()
        {
            var target = new QueryBuilder();
            var result = target.ParseQuery(@"id:a");

            Assert.AreEqual(1, result.Keys.Count);
            Assert.AreEqual(0, result.FreeText.Count);
            Assert.IsTrue(result.Keys.ContainsKey("id"));
            Assert.AreEqual("a", result.Keys["id"]);
        }

        [TestMethod]
        public void TestMethod8()
        {
            var target = new QueryBuilder();
            var result = target.ParseQuery(@"id:a+b");

            Assert.AreEqual(1, result.Keys.Count);
            Assert.AreEqual(0, result.FreeText.Count);
            Assert.IsTrue(result.Keys.ContainsKey("id"));
            Assert.AreEqual("a b", result.Keys["id"]);
        }

        [TestMethod]
        public void TestMethod9()
        {
            var target = new QueryBuilder();
            var result = target.ParseQuery(@"id:a b");

            Assert.AreEqual(1, result.Keys.Count);
            Assert.AreEqual(1, result.FreeText.Count);
            Assert.IsTrue(result.Keys.ContainsKey("id"));
            Assert.AreEqual("a", result.Keys["id"]);
            Assert.AreEqual("b", result.FreeText[0]);
        }

        [TestMethod]
        public void TestMethod10()
        {
            var target = new QueryBuilder();
            var result = target.ParseQuery(@"iD:a");

            Assert.AreEqual(1, result.Keys.Count);
            Assert.AreEqual(0, result.FreeText.Count);
            Assert.IsTrue(result.Keys.ContainsKey("id"));
            Assert.AreEqual("a", result.Keys["id"]);
        }

        [TestMethod]
        public void TestMethod11()
        {
            var target = new QueryBuilder();
            var result = target.ParseQuery(@"id:a b ""c d"" e+f packageId:""g""");

            Assert.AreEqual(2, result.Keys.Count);
            Assert.AreEqual(3, result.FreeText.Count);
            Assert.IsTrue(result.Keys.ContainsKey("id"));
            Assert.IsTrue(result.Keys.ContainsKey("packageid"));
            Assert.AreEqual("a", result.Keys["id"]);
            Assert.AreEqual("g", result.Keys["packageid"]);
            Assert.AreEqual("b", result.FreeText[0]);
            Assert.AreEqual("c d", result.FreeText[1]);
            Assert.AreEqual("e f", result.FreeText[2]);
        }
    }
}
