using System;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol
{
    /// <summary>
    /// ravendb.server
    /// </summary>
    public interface IRegistrationService
    {
        /// <summary>
        /// Retrieve the list of registrations by id
        /// PATH: /{lowerId}/index.json
        /// The heuristic that nuget.org uses is as follows: 
        /// if there are 128 or more versions of a package, break the leaves into 
        /// pages of size 64. If there are less than 128 versions, inline all 
        /// leaves into the registration index.
        /// </summary>
        /// <param name="lowerId"></param>
        /// <param name="withSemVer2"></param>
        /// <returns></returns>
        RegistrationIndex IndexPage(Guid repoId,string lowerId, string semVerLeve);

        /// <summary>
        ///  Retrieve the list of registrations by id paged between version
        /// PATH: /{lowerId}/page/{versionFrom}/{versionTo}.json
        /// </summary>
        /// <param name="lowerId"></param>
        /// <param name="versionFrom"></param>
        /// <param name="versionTo"></param>
        /// <param name="withSemVer2"></param>
        /// <returns></returns>
        RegistrationPage SinglePage(Guid repoId,string lowerId, string versionFrom, string versionTo, string semVerLeve);


        /// <summary>
        ///  Retrieve the registrations by id paged between version
        /// PATH: /{lowerId}/{version}/{loweridVerion}.json
        /// </summary>
        /// <param name="lowerId"></param>
        /// <param name="version"></param>
        /// <param name="loweridVersion"></param>
        /// <param name="withSemVer2"></param>
        /// <returns></returns>
        RegistrationLastLeaf Leaf(Guid repoId,string lowerId, string version, string loweridVersion, string semVerLeve);
    }
}
