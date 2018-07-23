using Newtonsoft.Json;

namespace NugetProtocol
{
    public class AutocompleteContext
    {
        public AutocompleteContext()
        {

        }
        public AutocompleteContext(string ovocab)
        {
            OVocab = ovocab;
        }

        [JsonProperty("@vocab")]
        public string OVocab { get; }
    }
}
