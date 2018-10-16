using System.Collections.Generic;

namespace Knapcode.NuGetTools.Logic
{
    public interface IFrameworkList
    {
        IReadOnlyList<FrameworkListItem> GetItems();
        IReadOnlyList<string> DotNetFrameworkNames { get; }
        IReadOnlyList<string> ShortFolderNames { get; }
        IReadOnlyList<string> Identifiers { get; }
    }
}
