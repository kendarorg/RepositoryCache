using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public interface IMavenTreeRepository : IRepository<MavenTreeItem>
    {
        MavenTreeItem GetAllChild(string[] group, string artifactId, string version);
    }
}
