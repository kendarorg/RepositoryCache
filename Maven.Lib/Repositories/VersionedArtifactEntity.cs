using System;
using Repositories;

namespace Maven.Repositories
{
    public class VersionedArtifactEntity : SearchableArtifact
    {
        public bool IsSnapshot { get; set; }
    }
}