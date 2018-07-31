using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol.Apis
{
    public interface IMavenSearch
    {
        SearchResult Search(Guid repoId, SearchParam param);
    }
}
