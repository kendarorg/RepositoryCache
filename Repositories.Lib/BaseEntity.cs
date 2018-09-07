using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            Id = Guid.Empty;
        }
        public Guid Id { get; set; }
    }
}
