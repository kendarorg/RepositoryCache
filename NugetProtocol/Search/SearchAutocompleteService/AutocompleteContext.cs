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

        public string OVocab { get; }
    }
}
