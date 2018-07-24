using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol
{
    public interface IServicesMapper
    {
        void Refresh();
        string From(Guid repoId, string resourceId, params string[] par);
        string FromSemver(Guid repoId, string resourceId,string semVerLevel, params string[] par);
        Dictionary<string, EntryPointDescriptor> GetVisibles(Guid id);
        string ToNuget(Guid repoId, string src);
        string FromNuget(Guid repoId, string src);
        int MaxRegistrationPages(Guid repoId);
        int MaxCatalogPages(Guid repoId);
    }
}
