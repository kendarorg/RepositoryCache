using System;
using System.Collections.Generic;

namespace Nuget.Services
{
    public class ParsedQuery
    {
        public ParsedQuery()
        {
            FreeText = new List<string>();
            Keys = new Dictionary<String, String>(StringComparer.InvariantCultureIgnoreCase);
        }
        public List<String> FreeText { get; set; }
        public Dictionary<string, string> Keys { get; set; }
    }
}
