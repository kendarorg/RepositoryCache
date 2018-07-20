using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Repositories
{
    public interface IRegistrationPageRepository : IRepository<RegistrationPageEntity>
    {
        IEnumerable<RegistrationPageEntity> GetAllByPackageId(Guid repoId,string lowerId);
    }
}
