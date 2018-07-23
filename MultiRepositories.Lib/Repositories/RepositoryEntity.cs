
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRepositories.Repositories
{
    
    public class RepositoryEntity : BaseEntity
    {
        public string Type { get; set; } //nuget/maven
        public bool Mirror { get; set; }
        public string Prefix { get; set; } //nuget.org/nuget.local
        public string Address { get; set; }
        public string Settings { get; set; }
        public string PackagesPath { get; set; }
    }
}
