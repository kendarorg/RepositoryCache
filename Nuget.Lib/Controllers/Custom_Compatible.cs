using MultiRepositories.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;
using Nuget.Services;
using MultiRepositories.Repositories;
using System.IO;
using Nuget.Framework;
using Nuget.Framework.FromNugetTools;

namespace Nuget.Controllers
{
    public class Custom_Compatible : RestAPI
    {
        
        private readonly Guid repoId;
        private readonly IFrameworkChecker _frameworkChecker;

        public Custom_Compatible(Guid repoId, IFrameworkChecker frameworkChecker, params string[] paths)
            : base(null, paths)
        {
            this.repoId = repoId;
            _frameworkChecker = frameworkChecker;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest localRequest)
        {
            
            var framework = localRequest.QueryParams["framework"];
            var shortVersion = localRequest.QueryParams.ContainsKey("short");

            if (!shortVersion)
            {
                var result = new CompatibilityResult();
                result.Base = _frameworkChecker.GetFrameworkDescriptor(framework);
                foreach (var compatible in _frameworkChecker.GetCompatibility(framework))
                {
                    result.CompatibleFrameworks.Add(_frameworkChecker.GetFrameworkDescriptor(compatible));
                }
                return JsonResponse(result);
            }
            else
            {
                var result = new CompatibilityResultShort();
                result.Base = _frameworkChecker.GetFrameworkDescriptor(framework);
                foreach (var compatible in _frameworkChecker.GetCompatibility(framework))
                {
                    result.CompatibleFrameworks.Add(_frameworkChecker.GetFrameworkDescriptor(compatible).ShortFolderName);
                }
                return JsonResponse(result);
            }

        }
        

    }

    public class CompatibilityResult
    {
        public CompatibilityResult()
        {
            CompatibleFrameworks = new List<FrameworkListItem>();
        }
        public List<FrameworkListItem> CompatibleFrameworks { get; set; }
        public FrameworkListItem Base { get; set; }
    }


    public class CompatibilityResultShort
    {
        public CompatibilityResultShort()
        {
            CompatibleFrameworks = new List<string>();
        }
        public List<string> CompatibleFrameworks { get; set; }
        public FrameworkListItem Base { get; set; }
    }
}
