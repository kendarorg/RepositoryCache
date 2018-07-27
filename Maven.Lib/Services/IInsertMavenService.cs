using Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Services
{
    public interface IInsertMavenService : ISingleton
    {
        void Insert(Guid repoId, string login, string password, byte[] data);
    }
}
