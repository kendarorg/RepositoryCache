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
        private int _maxReg;
        private int _maxCatalog;

        public ServicesMapperMock(string v, Guid repoId,int maxCat,int maxReg)
        {
            this._repoName = v;
            _repoId = repoId;
            _maxCatalog = maxCat;
            _maxReg = maxReg;
        }

        public string From(Guid repoId, string resourceId, params string[] par)
        {
            if (repoId != _repoId) throw new Exception();
            return _repoName + "/" + resourceId + "/" + string.Join("/", par).Trim('/');
        }

        public string FromNuget(Guid repoId, string src)
        {
            throw new NotImplementedException();
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

        public int MaxCatalogPages(Guid repoId)
        {
            return _maxCatalog;
        }

        public int MaxRegistrationPages(Guid repoId)
        {
            return _maxReg;
        }

        public void Refresh()
        {
            
        }

        public string ToNuget(Guid repoId, string src)
        {
            throw new NotImplementedException();
        }
    }
}
