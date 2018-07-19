using MultiRepositories.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRepositories
{
    public interface IRepositoryServiceProvider
    {
        void RegisterApi(RestAPI api);
    }
}
