using Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRepositories
{
    public interface IApiProvider : ISingleton
    {
        void Initialize(IRepositoryServiceProvider repositoryServiceProvider);
    }
}
