using System;
using Repositories;

namespace Nuget.Repositories
{
    public class NugetAssemblyGroup : BaseEntity
    {
        public string OwnerPackageId { get; set; }
        public string OwnerVersion { get; set; }
        public string TargetFramework { get; set; }
        public string AssemblyName { get; set; }
        public Guid RepositoryId { get;  set; }
    }
}