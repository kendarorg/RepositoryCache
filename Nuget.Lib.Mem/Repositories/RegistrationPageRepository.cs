using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;

namespace Nuget.Repositories
{
    public class RegistrationPageRepository : InMemoryRepository<RegistrationPageEntity>, IRegistrationPageRepository
    {
        public RegistrationPageRepository(AppProperties properties) : base(properties)
        {
        }

        public IEnumerable<RegistrationPageEntity> GetAllByPackageId(Guid repoId,string lowerId)
        {
            return GetAll().Where(a =>a.RepositoryId==repoId && a.PackageId == lowerId).OrderBy(a => a.PageIndex);
        }
    }
}
