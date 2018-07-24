using NugetProtocol;
using System;

namespace Nuget.Services
{
    public class InsertData
    {
        public PackageXml Nuspec { get; set; }
        public string Sha { get; set; }
        public string ShaAlgorithm { get; set; }
        public int Size { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid CommitId { get;  set; }
    }
}