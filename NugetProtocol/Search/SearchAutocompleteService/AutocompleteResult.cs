using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class AutocompleteResult
    {
        public AutocompleteResult()
        {

        }
        public AutocompleteResult(AutocompleteContext ocontext, int totalHits, DateTime lastReopen, string index,
            List<string> data)
        {
            OContext = ocontext;
            TotalHits = totalHits;
            LastReopen = lastReopen;
            Index = index;
            Data = data;
        }

        public AutocompleteContext OContext { get; }
        public int TotalHits { get; }
        public DateTime LastReopen { get; }
        public string Index { get; }
        public List<string> Data { get; }
    }
}
