﻿using System;

namespace Knapcode.NuGetTools.Logic
{
    [Flags]
    public enum FrameworkEnumerationOptions
    {
        None = 1 << 0,
        FrameworkNameProvider = 1 << 1,
        CommonFrameworks = 1 << 2,
        FrameworkMappings = 1 << 3,
        PortableFrameworkMappings = 1 << 4,
        SpecialFrameworks = 1 << 5,
        All = ~0
    }
}
