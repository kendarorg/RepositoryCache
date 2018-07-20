using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Repositories
{
    public interface IRegistrationRepository : IRepository<RegistrationEntity>
    {
        IEnumerable<RegistrationEntity> GetRange(Guid repoId, string lowerId, string versionFrom, string versionTo);
        RegistrationEntity GetSpecific(Guid repoId, string lowerId, string version);
    }
}
