
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Repositories
{
    public interface INugetAssembliesRepository : IRepository<NugetAssemblyGroup>
    {
        IEnumerable<NugetAssemblyGroup> GetGroups(Guid repoId, string idLower, string versionLower);
    }
}
