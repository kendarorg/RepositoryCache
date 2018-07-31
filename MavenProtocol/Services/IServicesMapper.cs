using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol
{
    public interface IServicesMapper
    {
        void Refresh();
        string From(Guid repoId, string resourceId, params string[] par);
        string ToMaven(Guid repoId, string src);
        string FromMvane(Guid repoId, string src);
        int MaxRegistrationPages(Guid repoId);
        int MaxCatalogPages(Guid repoId);
        int MaxQueryPage(Guid repoId);
    }
}
