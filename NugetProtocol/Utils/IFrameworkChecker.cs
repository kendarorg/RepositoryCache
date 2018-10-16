using System.Collections.Generic;
using Ioc;
using Nuget.Framework.FromNugetTools;

namespace Nuget.Framework
{
    public interface IFrameworkChecker:ISingleton
    {
        bool FrameworkExists(string fwName);
        string GetShortFolderName(string dotNetFrameworkName);
        string GetDotNetFrameworkName(string dotNetFrameworkName);
        IEnumerable<string> GetCompatibility(string dotNetFrameworkNames);
        FrameworkListItem GetFrameworkDescriptor(string dotNetFrameworkNames);
        void Add(object nn, string tf);
    }
}