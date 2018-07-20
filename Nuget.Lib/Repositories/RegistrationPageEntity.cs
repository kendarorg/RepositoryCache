
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Repositories
{
    public class RegistrationPageEntity : BaseEntity
    {
        public DateTime CommitTimestamp { get; set; }
        public Guid CommitId { get; set; }
        public Guid RepositoryId { get; set; }
        public string PackageId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public int Count { get; set; }
        //Internal orderby
        public int PageIndex { get; set; }
    }
}
