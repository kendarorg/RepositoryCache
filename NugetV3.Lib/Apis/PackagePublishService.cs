using NugetProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetV3.Apis
{
    public class PackagePublishService : IPackagePublishService
    {
        public void Create(string nugetApiKey, byte[] data)
        {
            throw new NotImplementedException();
        }

        public void Delist(string nugetApiKey, string id, string version)
        {
            throw new NotImplementedException();
        }

        public void Relist(string nugetApiKey, string id, string version)
        {
            throw new NotImplementedException();
        }
    }
}
