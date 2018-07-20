using System;
using System.IO;
using SemVer;
#if !NETSTANDARD
using System.Runtime.Serialization.Formatters.Binary;
#endif
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Semver.Test
{
    /// <summary>
    ///  Based on https://github.com/maxhauser/semver
    /// </summary>
    [TestClass]
    public class SemverTests
    {
        [TestMethod]
        public void SemVerCompareTestWithStrings1()
        {
            Assert.IsTrue(SemVersion.Equals("1.0.0", "1"));
        }

        [TestMethod]
        public void SemVerCompareTestWithStrings2()
        {
            var v = new SemVersion(1, 0, 0);
            Assert.IsTrue(v < "1.1");
        }

        [TestMethod]
        public void SemVerCompareTestWithStrings3()
        {
            var v = new SemVersion(1, 2);
            Assert.IsTrue(v > "1.0.0");
        }

        [TestMethod]
        public void SemVerCreateVersionTest()
        {
            var v = new SemVersion(1, 2, 3, "a", "b");

            Assert.AreEqual(1, v.Major);
            Assert.AreEqual(2, v.Minor);
            Assert.AreEqual(3, v.Patch);
            Assert.AreEqual("a", v.Prerelease);
            Assert.AreEqual("b", v.Build);
        }

        [TestMethod]
        public void SemVerCreateVersionTestWithNulls()
        {
            var v = new SemVersion(1, 2, 3, null, null);

            Assert.AreEqual(1, v.Major);
            Assert.AreEqual(2, v.Minor);
            Assert.AreEqual(3, v.Patch);
            Assert.AreEqual("", v.Prerelease);
            Assert.AreEqual("", v.Build);
        }

        [TestMethod]
        public void SemVerCreateVersionTestWithSystemVersion1()
        {
            var nonSemanticVersion = new Version(0, 0);
            var v = new SemVersion(nonSemanticVersion);

            Assert.AreEqual(0, v.Major);
            Assert.AreEqual(0, v.Minor);
            Assert.AreEqual(0, v.Patch);
            Assert.AreEqual("", v.Build);
            Assert.AreEqual("", v.Prerelease);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SemVerCreateVersionTestWithSystemVersion2()
        {
            var v = new SemVersion(null);
        }

        [TestMethod]
        public void SemVerCreateVersionTestWithSystemVersion3()
        {
            var nonSemanticVersion = new Version(1, 2, 0, 3);
            var v = new SemVersion(nonSemanticVersion);

            Assert.AreEqual(1, v.Major);
            Assert.AreEqual(2, v.Minor);
            Assert.AreEqual(3, v.Patch);
            Assert.AreEqual("", v.Build);
            Assert.AreEqual("", v.Prerelease);
        }

        [TestMethod]
        public void SemVerCreateVersionTestWithSystemVersion4()
        {
            var nonSemanticVersion = new Version(1, 2, 4, 3);
            var v = new SemVersion(nonSemanticVersion);

            Assert.AreEqual(1, v.Major);
            Assert.AreEqual(2, v.Minor);
            Assert.AreEqual(3, v.Patch);
            Assert.AreEqual("4", v.Build);
            Assert.AreEqual("", v.Prerelease);
        }

        [TestMethod]
        public void SemVerParseTest1()
        {
            var version = SemVersion.Parse("1.2.45-alpha+nightly.23");

            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(45, version.Patch);
            Assert.AreEqual("alpha", version.Prerelease);
            Assert.AreEqual("nightly.23", version.Build);
        }

        [TestMethod]
        public void SemVerParseTest2()
        {
            var version = SemVersion.Parse("1");

            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(0, version.Minor);
            Assert.AreEqual(0, version.Patch);
            Assert.AreEqual("", version.Prerelease);
            Assert.AreEqual("", version.Build);
        }

        [TestMethod]
        public void SemVerParseTest3()
        {
            var version = SemVersion.Parse("1.2.45-alpha-beta+nightly.23.43-bla");

            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(45, version.Patch);
            Assert.AreEqual("alpha-beta", version.Prerelease);
            Assert.AreEqual("nightly.23.43-bla", version.Build);
        }

        [TestMethod]
        public void SemVerParseTest4()
        {
            var version = SemVersion.Parse("2.0.0+nightly.23.43-bla");

            Assert.AreEqual(2, version.Major);
            Assert.AreEqual(0, version.Minor);
            Assert.AreEqual(0, version.Patch);
            Assert.AreEqual("", version.Prerelease);
            Assert.AreEqual("nightly.23.43-bla", version.Build);
        }

        [TestMethod]
        public void SemVerParseTest5()
        {
            var version = SemVersion.Parse("2.0+nightly.23.43-bla");

            Assert.AreEqual(2, version.Major);
            Assert.AreEqual(0, version.Minor);
            Assert.AreEqual(0, version.Patch);
            Assert.AreEqual("", version.Prerelease);
            Assert.AreEqual("nightly.23.43-bla", version.Build);
        }

        [TestMethod]
        public void SemVerParseTest6()
        {
            var version = SemVersion.Parse("2.1-alpha");

            Assert.AreEqual(2, version.Major);
            Assert.AreEqual(1, version.Minor);
            Assert.AreEqual(0, version.Patch);
            Assert.AreEqual("alpha", version.Prerelease);
            Assert.AreEqual("", version.Build);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SemVerParseTest7()
        {
            SemVersion.Parse("ui-2.1-alpha");
        }

        [TestMethod]
        public void SemVerParseTestStrict1()
        {
            var version = SemVersion.Parse("1.3.4", true);

            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(3, version.Minor);
            Assert.AreEqual(4, version.Patch);
            Assert.AreEqual("", version.Prerelease);
            Assert.AreEqual("", version.Build);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SemVerParseTestStrict2()
        {
            SemVersion.Parse("1", true);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SemVerParseTestStrict3()
        {
            SemVersion.Parse("1.3", true);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SemVerParseTestStrict4()
        {
            SemVersion.Parse("1.3-alpha", true);
        }

        [TestMethod]
        public void SemVerTryParseTest1()
        {
            SemVersion.TryParse("1.2.45-alpha-beta+nightly.23.43-bla", out SemVersion v);
        }

        [TestMethod]
        public void SemVerTryParseTest2()
        {
            Assert.IsFalse(SemVersion.TryParse("ui-2.1-alpha", out SemVersion v));
        }

        [TestMethod]
        public void SemVerTryParseTest3()
        {
            Assert.IsFalse(SemVersion.TryParse("", out SemVersion v));
        }

        [TestMethod]
        public void SemVerTryParseTest4()
        {
            Assert.IsFalse(SemVersion.TryParse(null, out SemVersion v));
        }

        [TestMethod]
        public void SemVerTryParseTest5()
        {
            Assert.IsTrue(SemVersion.TryParse("1.2", out SemVersion v, false));
        }

        [TestMethod]
        public void SemVerTryParseTest6()
        {
            Assert.IsFalse(SemVersion.TryParse("1.2", out SemVersion v, true));
        }

        [TestMethod]
        public void SemVerToStringTest()
        {
            var version = new SemVersion(1, 2, 0, "beta", "dev-mha.120");

            Assert.AreEqual("1.2.0-beta+dev-mha.120", version.ToString());
        }

        [TestMethod]
        public void SemVerEqualTest1()
        {
            var v1 = new SemVersion(1, 2, build: "nightly");
            var v2 = new SemVersion(1, 2, build: "nightly");

            var r = v1.Equals(v2);
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerEqualTest2()
        {
            var v1 = new SemVersion(1, 2, prerelease: "alpha", build: "dev");
            var v2 = new SemVersion(1, 2, prerelease: "alpha", build: "dev");

            var r = v1.Equals(v2);
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerEqualTest3()
        {
            var v1 = SemVersion.Parse("1.2-nightly+dev");
            var v2 = SemVersion.Parse("1.2.0-nightly");

            var r = v1.Equals(v2);
            Assert.IsFalse(r);
        }

        [TestMethod]
        public void SemVerEqualTest4()
        {
            var v1 = SemVersion.Parse("1.2-nightly");
            var v2 = SemVersion.Parse("1.2.0-nightly2");

            var r = v1.Equals(v2);
            Assert.IsFalse(r);
        }

        [TestMethod]
        public void SemVerEqualTest5()
        {
            var v1 = SemVersion.Parse("1.2.1");
            var v2 = SemVersion.Parse("1.2.0");

            var r = v1.Equals(v2);
            Assert.IsFalse(r);
        }

        [TestMethod]
        public void SemVerEqualTest6()
        {
            var v1 = SemVersion.Parse("1.4.0");
            var v2 = SemVersion.Parse("1.2.0");

            var r = v1.Equals(v2);
            Assert.IsFalse(r);
        }

        [TestMethod]
        public void SemVerEqualByReferenceTest()
        {
            var v1 = SemVersion.Parse("1.2-nightly");

            var r = v1.Equals(v1);
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerCompareTest1()
        {
            var v1 = SemVersion.Parse("1.0.0");
            var v2 = SemVersion.Parse("2.0.0");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerCompareTest2()
        {
            var v1 = SemVersion.Parse("1.0.0-beta+dev.123");
            var v2 = SemVersion.Parse("1-beta+dev.123");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(0, r);
        }

        [TestMethod]
        public void SemVerCompareTest3()
        {
            var v1 = SemVersion.Parse("1.0.0-alpha+dev.123");
            var v2 = SemVersion.Parse("1-beta+dev.123");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerCompareTest4()
        {
            var v1 = SemVersion.Parse("1.0.0-alpha");
            var v2 = SemVersion.Parse("1.0.0");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerCompareTest5()
        {
            var v1 = SemVersion.Parse("1.0.0");
            var v2 = SemVersion.Parse("1.0.0-alpha");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(1, r);
        }

        [TestMethod]
        public void SemVerCompareTest6()
        {
            var v1 = SemVersion.Parse("1.0.0");
            var v2 = SemVersion.Parse("1.0.1-alpha");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerCompareTest7()
        {
            var v1 = SemVersion.Parse("0.0.1");
            var v2 = SemVersion.Parse("0.0.1+build.12");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerCompareTest8()
        {
            var v1 = SemVersion.Parse("0.0.1+build.13");
            var v2 = SemVersion.Parse("0.0.1+build.12.2");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(1, r);
        }

        [TestMethod]
        public void SemVerCompareTest9()
        {
            var v1 = SemVersion.Parse("0.0.1-13");
            var v2 = SemVersion.Parse("0.0.1-b");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerCompareTest10()
        {
            var v1 = SemVersion.Parse("0.0.1+uiui");
            var v2 = SemVersion.Parse("0.0.1+12");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(1, r);
        }

        [TestMethod]
        public void SemVerCompareTest11()
        {
            var v1 = SemVersion.Parse("0.0.1+bu");
            var v2 = SemVersion.Parse("0.0.1");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(1, r);
        }

        [TestMethod]
        public void SemVerCompareTest12()
        {
            var v1 = SemVersion.Parse("0.1.1+bu");
            var v2 = SemVersion.Parse("0.2.1");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerCompareTest13()
        {
            var v1 = SemVersion.Parse("0.1.1-gamma.12.87");
            var v2 = SemVersion.Parse("0.1.1-gamma.12.88");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerCompareTest14()
        {
            var v1 = SemVersion.Parse("0.1.1-gamma.12.87");
            var v2 = SemVersion.Parse("0.1.1-gamma.12.87.1");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerCompareTest15()
        {
            var v1 = SemVersion.Parse("0.1.1-gamma.12.87.99");
            var v2 = SemVersion.Parse("0.1.1-gamma.12.87.X");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerCompareTest16()
        {
            var v1 = SemVersion.Parse("0.1.1-gamma.12.87");
            var v2 = SemVersion.Parse("0.1.1-gamma.12.87.X");

            var r = v1.CompareTo(v2);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerCompareNullTest()
        {
            var v1 = SemVersion.Parse("0.0.1+bu");
            var r = v1.CompareTo(null);
            Assert.AreEqual(1, r);
        }

        [TestMethod]
        public void SemVerTestHashCode()
        {
            var v1 = SemVersion.Parse("1.0.0-1+b");
            var v2 = SemVersion.Parse("1.0.0-1+c");

            var h1 = v1.GetHashCode();
            var h2 = v2.GetHashCode();

            Assert.AreNotEqual(h1, h2);
        }

        [TestMethod]
        public void SemVerTestStringConversion()
        {
            SemVersion v = "1.0.0";
            Assert.AreEqual(1, v.Major);
        }

        [TestMethod]
        public void SemVerTestUntypedCompareTo()
        {
            var v1 = new SemVersion(1);
            var c = v1.CompareTo((object)v1);

            Assert.AreEqual(0, c);
        }

        [TestMethod]
        public void SemVerStaticEqualsTest1()
        {
            var v1 = new SemVersion(1, 2, 3);
            var v2 = new SemVersion(1, 2, 3);

            var r = SemVersion.Equals(v1, v2);
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerStaticEqualsTest2()
        {
            var r = SemVersion.Equals(null, null);
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerStaticEqualsTest3()
        {
            var v1 = new SemVersion(1);

            var r = SemVersion.Equals(v1, null);
            Assert.IsFalse(r);
        }

        [TestMethod]
        public void SemVerStaticCompareTest1()
        {
            var v1 = new SemVersion(1);
            var v2 = new SemVersion(2);

            var r = SemVersion.Compare(v1, v2);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerStaticCompareTest2()
        {
            var v1 = new SemVersion(1);

            var r = SemVersion.Compare(v1, null);
            Assert.AreEqual(1, r);
        }

        [TestMethod]
        public void SemVerStaticCompareTest3()
        {
            var v1 = new SemVersion(1);

            var r = SemVersion.Compare(null, v1);
            Assert.AreEqual(-1, r);
        }

        [TestMethod]
        public void SemVerStaticCompareTest4()
        {
            var r = SemVersion.Compare(null, null);
            Assert.AreEqual(0, r);
        }

        [TestMethod]
        public void SemVerEqualsOperatorTest()
        {
            var v1 = new SemVersion(1);
            var v2 = new SemVersion(1);

            var r = v1 == v2;
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerUnequalOperatorTest()
        {
            var v1 = new SemVersion(1);
            var v2 = new SemVersion(2);

            var r = v1 != v2;
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerGreaterOperatorTest()
        {
            var v1 = new SemVersion(1);
            var v2 = new SemVersion(2);

            var r = v2 > v1;
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerGreaterOperatorTest2()
        {
            var v1 = new SemVersion(1, 0, 0, "alpha");
            var v2 = new SemVersion(1, 0, 0, "rc");

            var r = v2 > v1;
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerGreaterOperatorTest3()
        {
            var v1 = new SemVersion(1, 0, 0, "-ci.1");
            var v2 = new SemVersion(1, 0, 0, "alpha");

            var r = v2 > v1;
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerGreaterOrEqualOperatorTest1()
        {
            var v1 = new SemVersion(1);
            var v2 = new SemVersion(1);

            var r = v1 >= v2;
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerGreaterOrEqualOperatorTest2()
        {
            var v1 = new SemVersion(2);
            var v2 = new SemVersion(1);

            var r = v1 >= v2;
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerLessOperatorTest()
        {
            var v1 = new SemVersion(1);
            var v2 = new SemVersion(2);

            var r = v1 < v2;
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerLessOperatorTest2()
        {
            var v1 = new SemVersion(1, 0, 0, "alpha");
            var v2 = new SemVersion(1, 0, 0, "rc");

            var r = v1 < v2;
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerLessOperatorTest3()
        {
            var v1 = new SemVersion(1, 0, 0, "-ci.1");
            var v2 = new SemVersion(1, 0, 0, "alpha");

            var r = v1 < v2;
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerLessOrEqualOperatorTest1()
        {
            var v1 = new SemVersion(1);
            var v2 = new SemVersion(1);

            var r = v1 <= v2;
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerLessOrEqualOperatorTest2()
        {
            var v1 = new SemVersion(1);
            var v2 = new SemVersion(2);

            var r = v1 <= v2;
            Assert.IsTrue(r);
        }

        [TestMethod]
        public void SemVerTestChangeMajor()
        {
            var v1 = new SemVersion(1, 2, 3, "alpha", "dev");
            var v2 = v1.Change(major: 5);

            Assert.AreEqual(5, v2.Major);
            Assert.AreEqual(2, v2.Minor);
            Assert.AreEqual(3, v2.Patch);
            Assert.AreEqual("alpha", v2.Prerelease);
            Assert.AreEqual("dev", v2.Build);
        }

        [TestMethod]
        public void SemVerTestChangeMinor()
        {
            var v1 = new SemVersion(1, 2, 3, "alpha", "dev");
            var v2 = v1.Change(minor: 5);

            Assert.AreEqual(1, v2.Major);
            Assert.AreEqual(5, v2.Minor);
            Assert.AreEqual(3, v2.Patch);
            Assert.AreEqual("alpha", v2.Prerelease);
            Assert.AreEqual("dev", v2.Build);
        }

        [TestMethod]
        public void SemVerTestChangePatch()
        {
            var v1 = new SemVersion(1, 2, 3, "alpha", "dev");
            var v2 = v1.Change(patch: 5);

            Assert.AreEqual(1, v2.Major);
            Assert.AreEqual(2, v2.Minor);
            Assert.AreEqual(5, v2.Patch);
            Assert.AreEqual("alpha", v2.Prerelease);
            Assert.AreEqual("dev", v2.Build);
        }

        [TestMethod]
        public void SemVerTestChangePrerelease()
        {
            var v1 = new SemVersion(1, 2, 3, "alpha", "dev");
            var v2 = v1.Change(prerelease: "beta");

            Assert.AreEqual(1, v2.Major);
            Assert.AreEqual(2, v2.Minor);
            Assert.AreEqual(3, v2.Patch);
            Assert.AreEqual("beta", v2.Prerelease);
            Assert.AreEqual("dev", v2.Build);
        }

#if !NETSTANDARD
        [TestMethod]
        public void SemVerTestSerialization()
        {
            var semVer = new SemVersion(1, 2, 3, "alpha", "dev");
            SemVersion semVerSerializedDeserialized;
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, semVer);
                ms.Position = 0;
                semVerSerializedDeserialized = (SemVersion)bf.Deserialize(ms);
            }
            Assert.AreEqual(semVer, semVerSerializedDeserialized);
        }
#endif
    }
}