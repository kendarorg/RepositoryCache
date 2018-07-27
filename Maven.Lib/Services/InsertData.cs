using System;

namespace Maven.Services
{
    internal class InsertData
    {
        public Guid CommitId { get; set; }
        public object Nuspec { get; set; }
        public object HashKey { get; set; }
        public string HashAlgorithm { get; set; }
        public object Size { get; set; }
        public Guid RepoId { get; set; }
    }
}