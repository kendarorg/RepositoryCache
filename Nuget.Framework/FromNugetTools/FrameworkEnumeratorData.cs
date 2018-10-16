﻿using System;
using System.Globalization;
using System.Text;

namespace Nuget.Framework.FromNugetTools
{
    public class FrameworkEnumeratorData<TFramework> : IEquatable<FrameworkEnumeratorData<TFramework>>, IComparable<FrameworkEnumeratorData<TFramework>>
        where TFramework : IFramework
    {
        public FrameworkEnumeratorData(TFramework framework)
        {
            Identifier = framework.Identifier;
            Version = framework.Version;
            Profile = framework.Profile;
            Framework = framework;
        }

        public string Identifier { get; }
        public Version Version { get; }
        public string Profile { get; }
        public TFramework Framework { get; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Profile))
            {
                return $"{Identifier},Version=v{GetDisplayVersion(Version)}";
            }
            else
            {
                return $"{Identifier},Version=v{GetDisplayVersion(Version)},Profile={Profile}";
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FrameworkEnumeratorData<TFramework>);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) + StringComparer.OrdinalIgnoreCase.GetHashCode(Identifier);
                hash = (13 * hash) + Version.GetHashCode();
                hash = (13 * hash) + StringComparer.OrdinalIgnoreCase.GetHashCode(Profile);
                return hash;
            }
        }

        public bool Equals(FrameworkEnumeratorData<TFramework> other)
        {
            if (other == null)
            {
                return false;
            }

            return StringComparer.OrdinalIgnoreCase.Equals(Identifier, other.Identifier) &&
                   Version == other.Version &&
                   StringComparer.OrdinalIgnoreCase.Equals(Profile, other.Profile);
        }

        public int CompareTo(FrameworkEnumeratorData<TFramework> other)
        {
            var frameworkCompare = StringComparer.OrdinalIgnoreCase.Compare(Identifier, other.Identifier);
            if (frameworkCompare != 0)
            {
                return frameworkCompare;
            }

            var versionCompare = Version.CompareTo(other.Version);
            if (versionCompare != 0)
            {
                return frameworkCompare;
            }

            return Profile.CompareTo(other.Profile);
        }

        private static string GetDisplayVersion(Version version)
        {
            var sb = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor));

            if (version.Build > 0
                || version.Revision > 0)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, ".{0}", version.Build);

                if (version.Revision > 0)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, ".{0}", version.Revision);
                }
            }

            return sb.ToString();
        }
    }
}
