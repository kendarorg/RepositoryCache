﻿using System;
#if !NETSTANDARD
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
#endif
using System.Text.RegularExpressions;

namespace SemVer
{
    /// <summary>
    /// Based on https://github.com/maxhauser/semver
    /// A semantic version implementation.
    /// Conforms to v2.0.0 of http://semver.org/
    /// </summary>
#if NETSTANDARD
    public sealed class JavaSemVersion : IComparable<JavaSemVersion>, IComparable
#else
    [Serializable]
    public sealed class JavaSemVersion : IComparable<JavaSemVersion>, IComparable, ISerializable
#endif
    {
        public bool IsPreRelease
        {
            get
            {
                return !(
                    string.IsNullOrWhiteSpace(this.Prerelease));
            }
        }
        static Regex parseEx =
            new Regex(@"^(?<major>\d+)" +
                @"(\.(?<minor>\d+))?" +
                @"(\.(?<patch>\d+))?" +
                @"(\.(?<extra>\d+))?" +
                @"(\-(?<pre>[0-9A-Za-z\-\.]+))?$",
#if NETSTANDARD
                RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);
#else
                RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
#endif

#if !NETSTANDARD
        /// <summary>
        /// Initializes a new instance of the <see cref="JavaSemVersion" /> class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private JavaSemVersion(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException("info");
            var semVersion = Parse(info.GetString("JavaSemVersion"));
            Major = semVersion.Major;
            Minor = semVersion.Minor;
            Patch = semVersion.Patch;
            Extra = semVersion.Extra;
            Prerelease = semVersion.Prerelease;
            IsSnapsot = semVersion.IsSnapsot;
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaSemVersion" /> class.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <param name="patch">The patch version.</param>
        /// <param name="prerelease">The prerelease version (eg. "alpha").</param>
        /// <param name="build">The build eg ("nightly.232").</param>
        public JavaSemVersion(int major, int minor = 0, int patch = 0, string prerelease = "", bool snapshot = false, int? extra = null)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            this.Extra = extra;

            this.Prerelease = prerelease ?? "";
            this.IsSnapsot = snapshot;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaSemVersion"/> class.
        /// </summary>
        /// <param name="version">The <see cref="System.Version"/> that is used to initialize 
        /// the Major, Minor, Patch and Build properties.</param>
        public JavaSemVersion(Version version)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            this.Major = version.Major;
            this.Minor = version.Minor;

            if (version.Revision >= 0)
            {
                this.Patch = version.Revision;
            }

            this.Prerelease = String.Empty;

            this.IsSnapsot = false;
        }

        /// <summary>
        /// Parses the specified string to a semantic version.
        /// </summary>
        /// <param name="version">The version string.</param>
        /// <param name="strict">If set to <c>true</c> minor and patch version are required, else they default to 0.</param>
        /// <returns>The JavaSemVersion object.</returns>
        /// <exception cref="System.InvalidOperationException">When a invalid version string is passed.</exception>
        public static JavaSemVersion Parse(string version, bool strict = false)
        {
            var isSnapshot = false;
            var snap = version.ToUpperInvariant().IndexOf("-SNAPSHOT");

            if(snap>0)
            {
                version = version.Substring(0, snap);
                isSnapshot = true;
            }

            var match = parseEx.Match(version);
            if (!match.Success)
                throw new ArgumentException("Invalid version.", "version");

#if NETSTANDARD
            var major = int.Parse(match.Groups["major"].Value);
#else
            var major = int.Parse(match.Groups["major"].Value, CultureInfo.InvariantCulture);
#endif

            var minorMatch = match.Groups["minor"];
            int minor = 0;
            if (minorMatch.Success)
            {
#if NETSTANDARD
                minor = int.Parse(minorMatch.Value);
#else
                minor = int.Parse(minorMatch.Value, CultureInfo.InvariantCulture);
#endif
            }
            else if (strict)
            {
                throw new InvalidOperationException("Invalid version (no minor version given in strict mode)");
            }

            var patchMatch = match.Groups["patch"];
            int patch = 0;
            if (patchMatch.Success)
            {
#if NETSTANDARD
                patch = int.Parse(patchMatch.Value);
#else
                patch = int.Parse(patchMatch.Value, CultureInfo.InvariantCulture);
#endif
            }
            else if (strict)
            {
                throw new InvalidOperationException("Invalid version (no patch version given in strict mode)");
            }

            var extraMatch = match.Groups["extra"];
            int? extra = null;
            if (extraMatch.Success)
            {
                if (strict)
                {
                    throw new InvalidOperationException("Invalid version (no extra version given in strict mode)");
                }
#if NETSTANDARD
                patch = int.Parse(extraMatch.Value);
#else
                extra = int.Parse(extraMatch.Value, CultureInfo.InvariantCulture);
#endif
            }

            var prerelease = match.Groups["pre"].Value;
            var build = match.Groups["build"].Value;

            return new JavaSemVersion(major, minor, patch, prerelease, isSnapshot, extra);
        }

        /// <summary>
        /// Parses the specified string to a semantic version.
        /// </summary>
        /// <param name="version">The version string.</param>
        /// <param name="semver">When the method returns, contains a JavaSemVersion instance equivalent 
        /// to the version string passed in, if the version string was valid, or <c>null</c> if the 
        /// version string was not valid.</param>
        /// <param name="strict">If set to <c>true</c> minor and patch version are required, else they default to 0.</param>
        /// <returns><c>False</c> when a invalid version string is passed, otherwise <c>true</c>.</returns>
        public static bool TryParse(string version, out JavaSemVersion semver, bool strict = false)
        {
            try
            {
                semver = Parse(version, strict);
                return true;
            }
            catch (Exception)
            {
                semver = null;
                return false;
            }
        }

        /// <summary>
        /// Tests the specified versions for equality.
        /// </summary>
        /// <param name="versionA">The first version.</param>
        /// <param name="versionB">The second version.</param>
        /// <returns>If versionA is equal to versionB <c>true</c>, else <c>false</c>.</returns>
        public static bool Equals(JavaSemVersion versionA, JavaSemVersion versionB)
        {
            if (ReferenceEquals(versionA, null))
                return ReferenceEquals(versionB, null);
            return versionA.Equals(versionB);
        }

        /// <summary>
        /// Compares the specified versions.
        /// </summary>
        /// <param name="versionA">The version to compare to.</param>
        /// <param name="versionB">The version to compare against.</param>
        /// <returns>If versionA &lt; versionB <c>&lt; 0</c>, if versionA &gt; versionB <c>&gt; 0</c>,
        /// if versionA is equal to versionB <c>0</c>.</returns>
        public static int Compare(JavaSemVersion versionA, JavaSemVersion versionB)
        {
            if (ReferenceEquals(versionA, null))
                return ReferenceEquals(versionB, null) ? 0 : -1;
            return versionA.CompareTo(versionB);
        }

        public static bool IsGreater(string versionA, string versionB)
        {
            return JavaSemVersion.Parse(versionA) > JavaSemVersion.Parse(versionB);
        }

        /// <summary>
        /// Make a copy of the current instance with optional altered fields. 
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <param name="patch">The patch version.</param>
        /// <param name="prerelease">The prerelease text.</param>
        /// <param name="build">The build text.</param>
        /// <returns>The new version object.</returns>
        public JavaSemVersion Change(int? major = null, int? minor = null, int? patch = null,
            string prerelease = null, bool snapshot = false, int? extra = null)
        {
            return new JavaSemVersion(
                major ?? this.Major,
                minor ?? this.Minor,
                patch ?? this.Patch,
                prerelease ?? this.Prerelease,
                snapshot,
                extra ?? this.Extra);
        }

        /// <summary>
        /// Gets the major version.
        /// </summary>
        /// <value>
        /// The major version.
        /// </value>
        public int Major { get; private set; }

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        /// <value>
        /// The minor version.
        /// </value>
        public int Minor { get; private set; }

        /// <summary>
        /// Gets the patch version.
        /// </summary>
        /// <value>
        /// The patch version.
        /// </value>
        public int Patch { get; private set; }

        /// <summary>
        /// Gets the old build version.
        /// </summary>
        /// <value>
        /// The old build version.
        /// </value>
        public int? Extra { get; private set; }

        /// <summary>
        /// Gets the pre-release version.
        /// </summary>
        /// <value>
        /// The pre-release version.
        /// </value>
        public string Prerelease { get; private set; }

        public bool IsSnapsot { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var version = "" + Major + "." + Minor + "." + Patch;
            if (Extra != null)
                version += "." + Extra;
            if (!String.IsNullOrEmpty(Prerelease))
                version += "-" + Prerelease;
            if (IsSnapsot)
                version += "-SNAPSHOT";
            return version;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates 
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the 
        /// other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. 
        /// The return value has these meanings: Value Meaning Less than zero 
        ///  This instance precedes <paramref name="obj" /> in the sort order. 
        ///  Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. i
        ///  Greater than zero This instance follows <paramref name="obj" /> in the sort order.
        /// </returns>
        public int CompareTo(object obj)
        {
            return CompareTo((JavaSemVersion)obj);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates 
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the 
        /// other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. 
        /// The return value has these meanings: Value Meaning Less than zero 
        ///  This instance precedes <paramref name="other" /> in the sort order. 
        ///  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. i
        ///  Greater than zero This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public int CompareTo(JavaSemVersion other)
        {
            if (ReferenceEquals(other, null))
                return 1;

            var r = this.CompareByPrecedence(other);
            if (r != 0)
                return r;

            if (this.IsSnapsot && !other.IsSnapsot) return -1;
            if (!this.IsSnapsot && other.IsSnapsot) return 1;
            return 0;
        }

        /// <summary>
        /// Compares to semantic versions by precedence. This does the same as a Equals, but ignores the build information.
        /// </summary>
        /// <param name="other">The semantic version.</param>
        /// <returns><c>true</c> if the version precedence matches.</returns>
        public bool PrecedenceMatches(JavaSemVersion other)
        {
            return CompareByPrecedence(other) == 0;
        }

        /// <summary>
        /// Compares to semantic versions by precedence. This does the same as a Equals, but ignores the build information.
        /// </summary>
        /// <param name="other">The semantic version.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. 
        /// The return value has these meanings: Value Meaning Less than zero 
        ///  This instance precedes <paramref name="other" /> in the version precedence.
        ///  Zero This instance has the same precedence as <paramref name="other" />. i
        ///  Greater than zero This instance has creater precedence as <paramref name="other" />.
        /// </returns>
        public int CompareByPrecedence(JavaSemVersion other)
        {
            if (ReferenceEquals(other, null))
                return 1;

            var r = this.Major.CompareTo(other.Major);
            if (r != 0) return r;

            r = this.Minor.CompareTo(other.Minor);
            if (r != 0) return r;

            r = this.Patch.CompareTo(other.Patch);
            if (r != 0) return r;

            var lExtra = this.Extra ?? 0;
            var rExtra = other.Extra ?? 0;
            r = lExtra.CompareTo(rExtra);
            if (r != 0) return r;

            r = CompareComponent(this.Prerelease, other.Prerelease, true);
            if (r == 0)
            {
                if (this.IsSnapsot && !other.IsSnapsot) return -1;
                if (!this.IsSnapsot && other.IsSnapsot) return 1;
            }
            return r;
        }

        static int CompareComponent(string a, string b, bool lower = false)
        {
            var aEmpty = String.IsNullOrEmpty(a);
            var bEmpty = String.IsNullOrEmpty(b);
            if (aEmpty && bEmpty)
                return 0;

            if (aEmpty)
                return lower ? 1 : -1;
            if (bEmpty)
                return lower ? -1 : 1;

            var aComps = a.Split('.');
            var bComps = b.Split('.');

            var minLen = Math.Min(aComps.Length, bComps.Length);
            for (int i = 0; i < minLen; i++)
            {
                var ac = aComps[i];
                var bc = bComps[i];
                int anum, bnum;
                var isanum = Int32.TryParse(ac, out anum);
                var isbnum = Int32.TryParse(bc, out bnum);
                int r;
                if (isanum && isbnum)
                {
                    r = anum.CompareTo(bnum);
                    if (r != 0) return anum.CompareTo(bnum);
                }
                else
                {
                    if (isanum)
                        return -1;
                    if (isbnum)
                        return 1;
                    r = String.CompareOrdinal(ac, bc);
                    if (r != 0)
                        return r;
                }
            }

            return aComps.Length.CompareTo(bComps.Length);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var other = (JavaSemVersion)obj;

            return this.Major == other.Major &&
                this.Minor == other.Minor &&
                this.Patch == other.Patch &&
                this.Extra == other.Extra &&
                string.Equals(this.Prerelease, other.Prerelease, StringComparison.Ordinal) &&
                this.IsSnapsot==other.IsSnapsot;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = this.Major.GetHashCode();
                result = result * 31 + this.Minor.GetHashCode();
                result = result * 31 + this.Patch.GetHashCode();
                result = result * 31 + this.Prerelease.GetHashCode();
                if (Extra != null)
                {
                    result = result * 31 + this.Extra.Value.GetHashCode();
                }
                result = result * 31 + this.IsSnapsot.GetHashCode();
                return result;
            }
        }

#if !NETSTANDARD
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException("info");
            info.AddValue("JavaSemVersion", ToString());
        }
#endif

        /// <summary>
        /// Implicit conversion from string to JavaSemVersion.
        /// </summary>
        /// <param name="version">The semantic version.</param>
        /// <returns>The JavaSemVersion object.</returns>
        public static implicit operator JavaSemVersion(string version)
        {
            return JavaSemVersion.Parse(version);
        }

        /// <summary>
        /// The override of the equals operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator ==(JavaSemVersion left, JavaSemVersion right)
        {
            return JavaSemVersion.Equals(left, right);
        }

        /// <summary>
        /// The override of the un-equal operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is not equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator !=(JavaSemVersion left, JavaSemVersion right)
        {
            return !JavaSemVersion.Equals(left, right);
        }

        /// <summary>
        /// The override of the greater operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is greater than right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator >(JavaSemVersion left, JavaSemVersion right)
        {
            return JavaSemVersion.Compare(left, right) > 0;
        }

        /// <summary>
        /// The override of the greater than or equal operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is greater than or equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator >=(JavaSemVersion left, JavaSemVersion right)
        {
            return left == right || left > right;
        }

        /// <summary>
        /// The override of the less operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is less than right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator <(JavaSemVersion left, JavaSemVersion right)
        {
            return JavaSemVersion.Compare(left, right) < 0;
        }

        /// <summary>
        /// The override of the less than or equal operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is less than or equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator <=(JavaSemVersion left, JavaSemVersion right)
        {
            return left == right || left < right;
        }
    }
}
