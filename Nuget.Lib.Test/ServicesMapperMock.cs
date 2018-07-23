using NugetProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Lib.Test
{
    class ServicesMapperMock : IServicesMapper
    {
        private string _repoName;
        private readonly Guid _repoId;

        public ServicesMapperMock(string v, Guid repoId)
        {
            this._repoName = v;
            _repoId = repoId;
        }

        public string From(Guid repoId, string resourceId, params string[] par)
        {
            if (repoId != _repoId) throw new Exception();
            return _repoName + "/" + resourceId + "/" + string.Join("/", par).Trim('/');
        }

        public string FromSemver(Guid repoId, string resourceId, string semVerLevel, params string[] par)
        {
            if (repoId != _repoId) throw new Exception();
            return _repoName + "/" + resourceId + ":" + (semVerLevel ?? "") + "/" + string.Join("/", par).Trim('/');
        }

        public Dictionary<string, EntryPointDescriptor> GetVisibles(Guid id)
        {
            return new Dictionary<string, EntryPointDescriptor>();
        }

        public void Refresh()
        {
            
        }
    }
}
