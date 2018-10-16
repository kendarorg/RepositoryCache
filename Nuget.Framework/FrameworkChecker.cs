using System;
using System.Collections.Generic;
using System.Linq;
using Nuget.Framework.FromNuget;
using Nuget.Framework.FromNugetTools;

namespace Nuget.Framework
{
    public class FrameworkChecker : IFrameworkChecker
    {
        private readonly IFrameworkMappings _dfm;
        private readonly IFrameworkCompatibilityProvider _fcc;
        private readonly Dictionary<string, NuGetFramework> _netFrameworkNames;
        private readonly Dictionary<string, NuGetFramework> _shortFolderNames;
        private readonly Dictionary<string, List<NuGetFramework>> _compatibility;
        private Dictionary<string, NuGetFramework> _targetFrameworks;
        private Dictionary<string, NuGetFramework> _profiles;


        public FrameworkChecker()
        {
            _dfm = DefaultFrameworkMappings.Instance;
            _fcc = DefaultCompatibilityProvider.Instance;
            _netFrameworkNames = new Dictionary<string, NuGetFramework>(StringComparer.InvariantCultureIgnoreCase);
            _profiles = new Dictionary<string, NuGetFramework>(StringComparer.InvariantCultureIgnoreCase);
            _shortFolderNames = new Dictionary<string, NuGetFramework>(StringComparer.InvariantCultureIgnoreCase);
            _targetFrameworks = new Dictionary<string, NuGetFramework>(StringComparer.InvariantCultureIgnoreCase);
            _compatibility = new Dictionary<string, List<NuGetFramework>>(StringComparer.InvariantCultureIgnoreCase);
            SetupCompatibilityMap();
        }

        private void SetupCompatibilityMap()
        {
            var enumerator = new FrameworkEnumerator();
            var frameworkList = new FrameworkList<FromNugetTools.dd.Framework>(enumerator);

            foreach (var item in frameworkList.GetItems())
            {
                var fw = new NuGetFramework(item.Identifier, item.Version, item.Profile);
                _netFrameworkNames[item.DotNetFrameworkName] = fw;
                _shortFolderNames[item.ShortFolderName] = fw;
                if (!fw.HasProfile)
                {
                    var shortName = fw.Framework + fw.Version.Major + "." + fw.Version.Minor;
                    if (fw.Version.Revision > 0 || fw.Version.Build > 0)
                    {
                        shortName += "." + fw.Version.Build;
                        if (fw.Version.Revision > 0)
                        {
                            shortName += "." + fw.Version.Revision;
                        }
                    }

                    _targetFrameworks[shortName] = fw;
                }
                else
                {
                    _profiles[fw.Profile] = fw;
                }
            }
            foreach (var fwMain in _netFrameworkNames.Values.ToList())
            {
                _compatibility[fwMain.DotNetFrameworkName]=new List<NuGetFramework>();
                foreach (var fw in _netFrameworkNames.Values.ToList())
                {
                    if (_fcc.IsCompatible(fwMain, fw))
                    {
                        _compatibility[fwMain.DotNetFrameworkName].Add(fw);
                    }
                }
            }
        }

        public bool FrameworkExists(string fwName)
        {
            var profile = BuildProfile(fwName);
            return _netFrameworkNames.ContainsKey(fwName) || _shortFolderNames.ContainsKey(fwName)||
                _targetFrameworks.ContainsKey(fwName)||_profiles.ContainsKey(profile);
        }

        public string GetShortFolderName(string dotNetFrameworkName)
        {
            if (_netFrameworkNames.ContainsKey(dotNetFrameworkName))
            {
                return _netFrameworkNames[dotNetFrameworkName].GetShortFolderName();
            }
            else if (_shortFolderNames.ContainsKey(dotNetFrameworkName))
            {
                return _shortFolderNames[dotNetFrameworkName].GetShortFolderName();
            }
            else if (_targetFrameworks.ContainsKey(dotNetFrameworkName))
            {
                return _targetFrameworks[dotNetFrameworkName].GetShortFolderName();
            }
            else
            {
                var profile = BuildProfile(dotNetFrameworkName);
                if (_profiles.ContainsKey(profile))
                {
                    return _profiles[profile].GetShortFolderName();
                }
            }

            return null;

        }


        public string GetDotNetFrameworkName(string dotNetFrameworkName)
        {
            if (_netFrameworkNames.ContainsKey(dotNetFrameworkName))
            {
                return _netFrameworkNames[dotNetFrameworkName].DotNetFrameworkName;
            }
            else if (_shortFolderNames.ContainsKey(dotNetFrameworkName))
            { 
                return _shortFolderNames[dotNetFrameworkName].DotNetFrameworkName;
            }
            else if (_targetFrameworks.ContainsKey(dotNetFrameworkName))
            {
                return _targetFrameworks[dotNetFrameworkName].DotNetFrameworkName;
            }
            else
            {
                var profile = BuildProfile(dotNetFrameworkName);
                if (_profiles.ContainsKey(profile))
                {
                    return _profiles[profile].DotNetFrameworkName;
                }
            }

            return null;
        }

        public IEnumerable<string> GetCompatibility(string dotNetFrameworkNames)
        {
            dotNetFrameworkNames = GetDotNetFrameworkName(dotNetFrameworkNames);
            foreach (var item in _compatibility[dotNetFrameworkNames])
            {
                yield return item.GetShortFolderName();
            }
        }

        public void Add(object nn, string tf)
        {
            var fw = (NuGetFramework) nn;
            _netFrameworkNames[fw.DotNetFrameworkName] = fw;
            _shortFolderNames[fw.GetShortFolderName()] = fw;
            if (!fw.HasProfile)
            {
                var shortName = fw.Framework + fw.Version.Major + "." + fw.Version.Minor;
                if (fw.Version.Revision > 0 || fw.Version.Build > 0)
                {
                    shortName += "." + fw.Version.Build;
                    if (fw.Version.Revision > 0)
                    {
                        shortName += "." + fw.Version.Revision;
                    }
                }

                _targetFrameworks[shortName] = fw;
            }
            else
            {
                _profiles[fw.Profile] = fw;
            }
        }

        private string BuildProfile(string dotNetFrameworkName)
        {
            var spl = dotNetFrameworkName.Split('-').Select(a=>a.Trim());
            foreach (var item in spl)
            {
                if (item.ToLowerInvariant().StartsWith("profile"))
                {
                    return item;
                }
            }

            return "!NOTFOUND!";
        }
    }
}