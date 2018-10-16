using System;

namespace Nuget.Framework.FromNugetTools
{
    public class Framework : MarshalByRefObject, IFramework
    {
        private readonly IFrameworkApi _api;

        public Framework(object nuGetFramework, IFrameworkApi api)
        {
            NuGetFramework = nuGetFramework;
            _api = api;
        }

        public object NuGetFramework { get; }
        public string DotNetFrameworkName => _api.GetDotNetFrameworkName(NuGetFramework);
        public string ShortFolderName => _api.GetShortFolderName(NuGetFramework);
        public string Identifier => _api.GetIdentifer(NuGetFramework);
        public System.Version Version => _api.GetVersion(NuGetFramework);
        public string Profile => _api.GetProfile(NuGetFramework);
    }
}
