
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Repositories
{
    public class RegistrationEntity : BaseEntity
    {
        public DateTime CommitTimestamp { get; set; }
        public Guid CommitId { get; set; }
        public Guid RepositoryId { get; set; }
        public string PackageId { get; set; }
        public string Version { get; set; }
    }
}
