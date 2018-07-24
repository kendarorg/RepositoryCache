using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SemVer.Test
{
    [TestClass]
    public class ExtraTests
    {
        [TestMethod]
        public void SemVerAllowTrailingZeroes()
        {
            var version = SemVersion.Parse("01.02.045-alpha+nightly.23");

            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(45, version.Patch);
            Assert.AreEqual("alpha", version.Prerelease);
            Assert.AreEqual("nightly.23", version.Build);
        }

        [TestMethod]
        public void SemVerFillMissingParts()
        {
            var version = SemVersion.Parse("1-alpha+nightly.23");

            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(0, version.Minor);
            Assert.AreEqual(0, version.Patch);
            Assert.AreEqual("alpha", version.Prerelease);
            Assert.AreEqual("nightly.23", version.Build);
        }

        [TestMethod]
        public void SemVerShouldAccept4Parts()
        {
            var version = SemVersion.Parse("1.2.3.4-alpha+nightly.23");

            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(3, version.Patch);
            Assert.AreEqual(4, version.Extra);
            Assert.AreEqual("alpha", version.Prerelease);
            Assert.AreEqual("nightly.23", version.Build);
        }

        [TestMethod]
        public void SemVerShouldFillData()
        {
            var version = SemVersion.Parse("1.2.3.04-alpha+nightly.23");

            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(3, version.Patch);
            Assert.AreEqual(4, version.Extra);
            Assert.AreEqual("alpha", version.Prerelease);
            Assert.AreEqual("nightly.23", version.Build);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SemVerShouldNotAccept4PartsOnStrict()
        {
            var version = SemVersion.Parse("1.2.3.4-alpha+nightly.23", true);
        }

        [TestMethod]
        public void SemVerCompareTest1()
        {
            var v1 = SemVersion.Parse("1.0.0.1");
            var v2 = SemVersion.Parse("1.0.0.2");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerCompareTest2()
        {
            var v1 = SemVersion.Parse("1.0.0");
            var v2 = SemVersion.Parse("1.0.0.1");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(-1, r);
        }


        [TestMethod]
        public void SemVerCompareTest3()
        {
            var v1 = SemVersion.Parse("1.0.0-alpha");
            var v2 = SemVersion.Parse("1.0.0.1-alpha");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerToStringTest()
        {
            var version = new SemVersion(1, 2, 0, "beta", "dev-mha.120", 2);

            Assert.AreEqual("1.2.0.2-beta+dev-mha.120", version.ToString());
        }
    }
}
