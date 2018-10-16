using System.Collections.Generic;

namespace Nuget.Framework.FromNugetTools
{
    public interface IFrameworkEnumerator<TFramework> where TFramework : IFramework
    {
        IEnumerable<FrameworkEnumeratorData<TFramework>> Enumerate(FrameworkEnumerationOptions options);
        IEnumerable<FrameworkEnumeratorData<TFramework>> Expand(IEnumerable<FrameworkEnumeratorData<TFramework>> frameworks, FrameworkExpansionOptions options);
    }
}