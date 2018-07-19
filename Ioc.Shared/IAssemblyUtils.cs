using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ioc
{
    public interface IAssemblyUtils : ISingleton
    {
        IEnumerable<Type> LoadAlltypes(params Type[] interfaces);
    }
}
