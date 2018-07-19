using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol
{
    public interface IPackageBaseAddressService
    {
        /// <summary>
        /// Download the package
        /// GET: /{lowerId}/{lowerVersion}/{lowerIdlowerVersion}.nupkg"
        /// </summary>
        /// <param name="lowerIdlowerVersion"></param>
        /// <returns></returns>
        byte[] GetNupkg(Guid repoId,string lowerIdlowerVersion);

        /// <summary>
        /// Retrieve list of versions
        /// GET: /{lowerId}/index.json
        /// </summary>
        /// <param name="lowerId"></param>
        /// <returns></returns>
        VersionsResult GetVersions(Guid repoId,string lowerId);

        /// <summary>
        /// Download the package
        /// GET: /{lowerId}/{lowerVersion}/{lowerId}.nuspec"
        /// </summary>
        /// <param name="lowerId></param>
        /// <param name="lowerVersion"></param>
        /// <returns></returns>
        string GetNuspec(Guid repoId,string lowerId,string  lowerVersion);
    }
}
