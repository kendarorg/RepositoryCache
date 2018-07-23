﻿using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;

namespace Nuget.Repositories
{
    public class RegistrationRepository : InMemoryRepository<RegistrationEntity>, IRegistrationRepository
    {
        public RegistrationRepository(AppProperties properties) : base(properties)
        {
        }

        public IEnumerable<RegistrationEntity> GetAllByPackageId(Guid repoId, string lowerId)
        {
            return GetAll().Where(a => a.RepositoryId == repoId).
                OrderBy(a=>a.Major).OrderBy(a => a.Minor).OrderBy(a => a.Patch).OrderBy(a => a.PreRelease);
        }

        public IEnumerable<RegistrationEntity> GetRange(Guid repoId, string lowerId, string versionFrom, string versionTo)
        {
            var from = SemVerParser.Parse(versionFrom);
            var to = SemVerParser.Parse(versionTo);
            foreach (var item in GetAll().Where(a => a.RepositoryId == repoId && a.PackageId == lowerId))
            {
                var currver = SemVerParser.Parse(item.Version);
                if (currver >= from && currver <= to)
                {
                    yield return item;
                }
            }
        }

        public RegistrationEntity GetSpecific(Guid repoId, string lowerId, string version)
        {
            return GetAll().FirstOrDefault(a => a.RepositoryId == repoId && a.PackageId == lowerId && a.Version == version);
        }
    }
}
