using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetProtocol
{
    public interface IPackagePublishService
    {
        /// <summary>
        /// Add package, verb PUT
        /// PATH: /
        /// </summary>
        /// <param name="nugetApiKey">X-NuGet-ApiKey in headers</param>
        /// <param name="data">multipart/form-data first item is body</param>
        void Create(Guid repoId,string nugetApiKey, byte[] data);

        /// <summary>
        /// Delist package, verb DELETE
        /// PATH: /{id}/{version}
        /// </summary>
        /// <param name="nugetApiKey">X-NuGet-ApiKey in headers</param>
        /// <param name="id"></param>
        /// <param name="version"></param>
        void Delist(Guid repoId,string nugetApiKey, string id, string version);

        /// <summary>
        /// Delist package, verb POST
        /// PATH: /{id}/{version}
        /// </summary>
        /// <param name="nugetApiKey">X-NuGet-ApiKey in headers</param>
        /// <param name="id"></param>
        /// <param name="version"></param>
        void Relist(Guid repoId,string nugetApiKey, string id, string version);
    }
}
