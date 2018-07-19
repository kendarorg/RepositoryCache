using System;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol
{

    public interface ISearchQueryService
    {
        /// <summary>
        /// Do not show unlisted results
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        QueryResult Query(Guid repoId,QueryModel query);
    }
}
