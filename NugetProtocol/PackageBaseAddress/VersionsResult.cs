﻿using System.Collections.Generic;

namespace NugetProtocol
{
    public class VersionsResult
    {
        public VersionsResult()
        {

        }
        public VersionsResult(List<string> versions)
        {
            Versions = versions;
        }

        public List<string> Versions { get; }
    }
}