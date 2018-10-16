using System;
using System.Collections.Generic;
using System.Linq;
using Knapcode.NuGetTools.Logic;
using Knapcode.NuGetTools.Logic.Direct;
using NuGet.Frameworks;

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


        public FrameworkChecker()
        {
            _dfm = DefaultFrameworkMappings.Instance;
            _fcc = DefaultCompatibilityProvider.Instance;
            _netFrameworkNames = new Dictionary<string, NuGetFramework>();
            _shortFolderNames = new Dictionary<string, NuGetFramework>();
            _targetFrameworks = new Dictionary<string, NuGetFramework>();
            _compatibility = new Dictionary<string, List<NuGetFramework>>();
            SetupCompatibilityMap();
        }

        private void SetupCompatibilityMap()
        {
            var enumerator = new FrameworkEnumerator();
            var frameworkList = new FrameworkList<Knapcode.NuGetTools.Logic.Direct.Wrappers.Framework>(enumerator);

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
                        shortName += "." + fw.Version.Revision;
                        if (fw.Version.Build > 0)
                        {
                            shortName += "." + fw.Version.Build;
                        }
                    }

                    _targetFrameworks[shortName] = fw;
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
            return _netFrameworkNames.ContainsKey(fwName) || _shortFolderNames.ContainsKey(fwName)||
                _targetFrameworks.ContainsKey(fwName);
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
            else
            {
                return _targetFrameworks[dotNetFrameworkName].GetShortFolderName();
            }

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
            else
            {
                return _targetFrameworks[dotNetFrameworkName].DotNetFrameworkName;
            }
        }

        public IEnumerable<string> GetCompatibility(string dotNetFrameworkNames)
        {
            dotNetFrameworkNames = GetDotNetFrameworkName(dotNetFrameworkNames);
            foreach (var item in _compatibility[dotNetFrameworkNames])
            {
                yield return item.GetShortFolderName();
            }
        }
    }
}