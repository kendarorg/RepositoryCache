using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SemVer.Test
{
    [TestClass]
    public class ExtraTests
    {
        [TestMethod]
        public void AllowTrailingZeroes()
        {
            var version = SemVersion.Parse("01.02.045-alpha+nightly.23");

            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(45, version.Patch);
            Assert.AreEqual("alpha", version.Prerelease);
            Assert.AreEqual("nightly.23", version.Build);
        }

        [TestMethod]
        public void FillMissingParts()
        {
            var version = SemVersion.Parse("1-alpha+nightly.23");

            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(0, version.Minor);
            Assert.AreEqual(0, version.Patch);
            Assert.AreEqual("alpha", version.Prerelease);
            Assert.AreEqual("nightly.23", version.Build);
        }
    }
}
