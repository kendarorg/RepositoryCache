using MavenProtocol.Apis;
using MultiRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Services
{
    public interface IRequestParser
    {
        MavenIndex Parse(SerializableRequest request);
    }
}
