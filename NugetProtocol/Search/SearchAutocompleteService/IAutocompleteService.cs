using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol
{
    public interface IAutocompleteService
    {
        /// <summary>
        /// Do not show unlisted results
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        AutocompleteResult Query(Guid repoId,QueryModel query);

        /// <summary>
        /// Retrieve the versions
        /// Note that the address MUST NOT FINISH WITH SLASH
        /// GET: ?id=newtonsoft.json&prerelease=true&semVerLevel=2.0.0
        /// </summary>
        /// <param name="id"></param>
        /// <param name="prerelease"></param>
        /// <param name="semVerLevel"></param>
        /// <returns></returns>
        AutocompleteResult QueryByPackage(Guid repoId,string id, bool prerelease =true, string semVerLevel = "1.0.0");
    }
}
