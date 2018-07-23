using Ioc;
using Nuget.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Services
{
    public class InsertNugetService : IInsertNugetService
    {
        public InsertNugetService(
            IQueryRepository  queryRepository, 
            IRegistrationRepository registrationRepository)
        {

        }
        public void Insert(Guid repoId, string nugetApiKey, byte[] data)
        {
            var commitId = Guid.NewGuid();
            var commitTimestamp = DateTime.UtcNow;
            throw new NotImplementedException();
        }
    }
}
