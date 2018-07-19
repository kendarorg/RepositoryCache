using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol
{

    public interface IIndexService
    {
        ServiceIndex Get(Guid repoId);
    }
}
