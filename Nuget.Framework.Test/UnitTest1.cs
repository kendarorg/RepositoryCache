using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using Knapcode.NuGetTools.Logic;
using Knapcode.NuGetTools.Logic.Direct;
using Knapcode.NuGetTools.Logic.Direct.Wrappers;
using NuGet.Frameworks;

namespace Nuget.Framework.Test
{
    [TestClass]
    public class UnitTest1
    {
       
        [TestMethod]
        public void TestMethod2()
        {
            var _fcc = DefaultCompatibilityProvider.Instance;
            var enumerator = new FrameworkEnumerator();
            var frameworkList = new FrameworkList<Knapcode.NuGetTools.Logic.Direct.Wrappers.Framework>(enumerator);
            var _identifiers = new Dictionary<string, NuGetFramework>();
            foreach (var item in frameworkList.GetItems())
            {
                var fw = new NuGetFramework(item.Identifier, item.Version, item.Profile);
                _identifiers[item.DotNetFrameworkName] = fw;
            }

           var  _compatibility = new Dictionary<string, List<NuGetFramework>>();
            foreach (var fwMain in _identifiers.Values.ToList())
            {
                _compatibility[fwMain.DotNetFrameworkName] = new List<NuGetFramework>();
                foreach (var fw in _identifiers.Values.ToList())
                {
                    if (_fcc.IsCompatible(fwMain, fw))
                    {
                        _compatibility[fwMain.DotNetFrameworkName].Add(fw);
                    }
                }
            }
            Console.WriteLine("Aaa");
        }
    }
}
