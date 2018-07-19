using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol
{
    public interface IReportAbuseService
    {
        /// <summary>
        /// Report abuse
        /// GET: /{id}/{version}/ReportAbuse
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="version"></param>
        void ReportAbuse(Guid repoId,string packageId, string version);
    }
}
