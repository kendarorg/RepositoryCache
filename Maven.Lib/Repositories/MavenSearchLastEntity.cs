using System;
using Repositories;

namespace Maven.Repositories
{
    public class MavenSearchLastEntity : MavenBaseSearchEntity
    {
        
        public string JsonPlugins { get; set; }
        public string VersionRelease { get; set; }
        public string VersionSnapshot { get; set; }
        public DateTime TimestampRelease { get; set; }
        public DateTime TimestampSnapshot { get; set; }
    }
}