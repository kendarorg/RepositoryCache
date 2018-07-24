using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol
{
    public interface ICatalogService
    {

        /// <summary>
        /// List all catalog pages
        /// GET: /index.json
        /// </summary>
        /// <returns></returns>
        CatalogIndex GetCatalog(Guid repoId);

        /// <summary>
        /// List all catalogs
        /// GET: /index{page}.json
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        CatalogPage GetCatalogPage(Guid repoId, int page);

        /// <summary>
        /// GET: /data/{timestamp}/{packageIdVersionLower}.json
        /// </summary>
        /// <param name="timestamp">2018.12.31.23.59.59</param>
        /// <param name="packageIdVersionLower"></param>
        /// <returns></returns>
        CatalogEntry GetPackageCatalog(Guid repoId,string timestamp, string packageIdVersionLower);

        /// <summary>
        /// Internal usage
        /// </summary>
        /// <param name="lowerId"></param>
        /// <param name="withSemVer2"></param>
        /// <param name="lowerVersion"></param>
        /// <returns></returns>
        IEnumerable<PackageDetail> GetPackageDetailsForRegistration(Guid repoId,string lowerId, string semVerLevel, params string[] lowerVersion);
    }
}
