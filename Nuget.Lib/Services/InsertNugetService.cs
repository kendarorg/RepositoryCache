using Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Services
{
    public class InsertNugetService : IInsertNugetService
    {
        public void Insert(Guid repoId, string nugetApiKey, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
