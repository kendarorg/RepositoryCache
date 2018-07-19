using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class SamplePackageBaseAddressService : IPackageBaseAddressService
    {
        public byte[] GetNupkg(Guid repoId,string lowerIdlowerVersion)
        {
            return new byte[] { };
        }

        public string GetNuspec(Guid repoId, string lowerId, string lowerVersion)
        {
            return string.Empty;
        }

        public VersionsResult GetVersions(Guid repoId, string lowerId)
        {
            return new VersionsResult(new List<string> { "1.0.0", "2.0.0" });
        }
    }
}
