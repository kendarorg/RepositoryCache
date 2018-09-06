using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MavenProtocol.Apis;

namespace MavenProtocol
{
    public interface IServicesMapper
    {
        void Refresh();
        int MaxRegistrationPages(Guid repoId);
        bool HasTimestampedSnapshot(Guid repoId);
        int MaxCatalogPages(Guid repoId);
        int MaxQueryPage(Guid repoId);
        string ToMaven(Guid id, MavenIndex idx, bool search);
    }
}
