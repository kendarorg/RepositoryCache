using System;
using System.Collections.Generic;

namespace NugetProtocol
{
    public class SampleAutocompleteService : IAutocompleteService
    {
        private IServicesMapper _servicesMapper = null;
        public AutocompleteResult Query(Guid repoId,QueryModel query)
        {
            return new AutocompleteResult(
                new AutocompleteContext(_servicesMapper.From(repoId,"*Schema")),
                250, DateTime.Now, Guid.NewGuid().ToString(),
                new List<string>
                {
                    "WindowsAzure.Storage",
                    "Kivii.Storages"
                }
                );
        }

        public AutocompleteResult QueryByPackage(Guid repoId,string id, bool prerelease = true, string semVerLevel = "1.0.0")
        {
            return new AutocompleteResult(
                new AutocompleteContext(_servicesMapper.From(repoId,"*Schema")),
                250, DateTime.Now, Guid.NewGuid().ToString(),
                new List<string>
                {
                    "1.0.0",
                    "1.0.1",
                    "1.2.0"
                }
                );
        }
    }
}
